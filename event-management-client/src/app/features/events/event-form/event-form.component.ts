import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';

import { EventService } from 'src/app/core/services/event.service';
import { Event as EventModel, EventStatus, EventType } from 'src/app/core/models/event.model';
import { ConfirmDialogComponent } from 'src/app/shared/components/confirm-dialog/confirm-dialog.component';
import { environment } from '@environments/environment';

@Component({
  selector: 'app-event-form',
  templateUrl: './event-form.component.html',
  styleUrls: ['./event-form.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatTableModule,
    MatChipsModule,
    MatDialogModule
  ]
})
export class EventFormComponent implements OnInit {
  eventForm: FormGroup;
  isLoading = false;
  isSubmitting = false;
  event: EventModel | null = null;
  editMode = false;
  displayedColumns: string[] = ['title', 'date', 'status', 'actions'];
  subEvents: EventModel[] = [];
  
  subEventsEnabled = environment.features.subEvents;
  eventsByTypeEnabled = environment.features.eventsByType;

  eventTypes = Object.entries(EventType)
    .filter(([key]) => isNaN(Number(key)))
    .map(([key, value]) => ({ name: key, value }));
  
  eventStatuses = Object.entries(EventStatus)
    .filter(([key]) => isNaN(Number(key)))
    .map(([key, value]) => ({ name: key, value }));
  
  parentEvents: EventModel[] = [];
  selectedFile: File | null = null;
  imagePreviewUrl: string | ArrayBuffer | null = null;
  
  constructor(
    private fb: FormBuilder,
    private eventService: EventService,
    private route: ActivatedRoute,
    private router: Router,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {
    this.eventForm = this.createEventForm();
  }

  ngOnInit(): void {
    this.isLoading = true;
    
    // Load parent events for dropdown (if needed)
    this.eventService.getEventsByType(EventType.Series).subscribe({
      next: (events) => {
        this.parentEvents = events;
        
        // Check if we're in edit mode
        const id = this.route.snapshot.paramMap.get('id');
        if (id) {
          this.editMode = true;
          this.loadEvent(parseInt(id, 10));
        } else {
          this.isLoading = false;
        }
      },
      error: (error) => {
        console.error('Error loading parent events', error);
        this.snackBar.open('Error loading form data. Please try again.', 'Close', {
          duration: 5000,
          panelClass: 'error-snackbar'
        });
        this.isLoading = false;
      }
    });
    
    // Set up listener for event type changes
    this.eventForm.get('type')?.valueChanges.subscribe(value => {
      const parentEventIdControl = this.eventForm.get('parentEventId');
      
      // If event type is SubEvent, make parentEventId required
      if (value === EventType.SubEvent) {
        parentEventIdControl?.setValidators([Validators.required]);
      } else {
        parentEventIdControl?.clearValidators();
        parentEventIdControl?.setValue(null);
      }
      parentEventIdControl?.updateValueAndValidity();
    });
  }

  createEventForm(): FormGroup {
    return this.fb.group({
      id: [0],
      title: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', Validators.required],
      startDate: [new Date(), Validators.required],
      endDate: [new Date(new Date().setDate(new Date().getDate() + 1)), Validators.required],
      venueName: ['', Validators.required],
      capacity: [100, [Validators.required, Validators.min(1)]],
      type: [EventType.Standalone, Validators.required],
      parentEventId: [null],
      status: [EventStatus.Draft, Validators.required],
      bannerImageUrl: ['']
    });
  }

  loadEvent(id: number): void {
    this.eventService.getEventById(id).subscribe({
      next: (event) => {
        this.event = event;
        this.populateFormWithEvent(event);
        
        // If it's a series event, load sub-events
        if (event.type === EventType.Series) {
          this.loadSubEvents(event.id);
        } else {
          this.isLoading = false;
        }
      },
      error: (error) => {
        console.error('Error loading event', error);
        this.snackBar.open('Error loading event details. Please try again.', 'Close', {
          duration: 5000,
          panelClass: 'error-snackbar'
        });
        this.isLoading = false;
        this.router.navigate(['/events']);
      }
    });
  }

  loadSubEvents(parentId: number): void {
    this.eventService.getSubEvents(parentId).subscribe({
      next: (subEvents) => {
        this.subEvents = subEvents;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading sub-events', error);
        this.snackBar.open('Error loading sub-events. Please try again.', 'Close', {
          duration: 5000,
          panelClass: 'error-snackbar'
        });
        this.isLoading = false;
      }
    });
  }

  populateFormWithEvent(event: EventModel): void {
    // Convert string dates to Date objects
    const startDate = new Date(event.startDate);
    const endDate = new Date(event.endDate);
    
    this.eventForm.patchValue({
      id: event.id,
      title: event.title,
      description: event.description,
      startDate: startDate,
      endDate: endDate,
      venueName: event.venueName,
      capacity: event.capacity,
      type: event.type,
      parentEventId: event.parentEventId,
      status: event.status,
      bannerImageUrl: event.bannerImageUrl
    });
    
    // Set image preview if available
    if (event.bannerImageUrl) {
      this.imagePreviewUrl = event.bannerImageUrl;
    }
  }

  onSubmit(): void {
    if (this.eventForm.invalid) {
      // Mark all fields as touched to trigger validation messages
      Object.keys(this.eventForm.controls).forEach(key => {
        this.eventForm.get(key)?.markAsTouched();
      });
      return;
    }
    
    this.isSubmitting = true;
    const eventData = { ...this.eventForm.value };
    
    // Handle file upload if there's a selected file
    if (this.selectedFile) {
      this.eventService.uploadImage(this.selectedFile).subscribe({
        next: (imageUrl) => {
          eventData.bannerImageUrl = imageUrl;
          this.  saveEvent(eventData);
        },
        error: (error) => {
          console.error('Error uploading image', error);
          this.snackBar.open('Error uploading image. Saving event without image.', 'Close', {
            duration: 5000
          });
          this.saveEvent(eventData);
        }
      });
    } else {
      this.saveEvent(eventData);
    }
  }

  saveEvent(eventData: EventModel): void {
    if (this.editMode) {
      this.eventService.updateEvent(eventData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.snackBar.open('Event updated successfully', 'Close', {
            duration: 3000
          });
          this.router.navigate(['/events']);
        },
        error: (error) => {
          console.error('Error updating event', error);
          this.snackBar.open('Error updating event. Please try again.', 'Close', {
            duration: 5000,
            panelClass: 'error-snackbar'
          });
          this.isSubmitting = false;
        }
      });
    } else {
      this.eventService.createEvent(eventData).subscribe({
        next: (id) => {
          this.isSubmitting = false;
          this.snackBar.open('Event created successfully', 'Close', {
            duration: 3000
          });
          this.router.navigate(['/events']);
        },
        error: (error) => {
          console.error('Error creating event', error);
          this.snackBar.open('Error creating event. Please try again.', 'Close', {
            duration: 5000,
            panelClass: 'error-snackbar'
          });
          this.isSubmitting = false;
        }
      });
    }
  }

  onFileSelected(event: Event): void {
    const fileInput = event.target as HTMLInputElement;
    if (fileInput.files && fileInput.files.length > 0) {
      this.selectedFile = fileInput.files[0];
      
      // Create a preview
      const reader = new FileReader();
      reader.onload = () => {
        this.imagePreviewUrl = reader.result;
      };
      reader.readAsDataURL(this.selectedFile);
    }
  }

  removeImage(): void {
    this.selectedFile = null;
    this.imagePreviewUrl = null;
    this.eventForm.patchValue({ bannerImageUrl: '' });
  }

  deleteSubEvent(id: number): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '350px',
      data: {
        title: 'Confirm Delete',
        message: 'Are you sure you want to delete this sub-event? This action cannot be undone.',
        confirmButtonText: 'Delete',
        cancelButtonText: 'Cancel'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.eventService.deleteEvent(id).subscribe({
          next: () => {
            this.loadSubEvents(this.event!.id);
            this.snackBar.open('Sub-event deleted successfully', 'Close', {
              duration: 3000
            });
          },
          error: (error) => {
            console.error('Error deleting sub-event', error);
            this.snackBar.open('Error deleting sub-event. Please try again.', 'Close', {
              duration: 5000,
              panelClass: 'error-snackbar'
            });
          }
        });
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/events']);
  }

  getEventTypeName(type: EventType): string {
    return EventType[type];
  }

  getEventStatusName(status: EventStatus): string {
    return EventStatus[status];
  }

  getStatusChipClass(status: EventStatus): string {
    switch (status) {
      case EventStatus.Draft:
        return 'status-draft';
      case EventStatus.Published:
        return 'status-published';
      case EventStatus.Canceled:
        return 'status-canceled';
      case EventStatus.Completed:
        return 'status-completed';
      default:
        return '';
    }
  }
}