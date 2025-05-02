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

import { EventService } from 'src/app/core/services/event.service';
import { Event as EventModel, EventStatus, EventType } from 'src/app/core/models/event.model';

@Component({
  selector: 'app-sub-event-form',
  templateUrl: './sub-event-form.component.html',
  styleUrls: ['./sub-event-form.component.scss'],
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
    MatSnackBarModule
  ]
})
export class SubEventFormComponent implements OnInit {
  subEventForm: FormGroup;
  isLoading = true;
  isSubmitting = false;
  parentEvent: EventModel | null = null;
  parentId: number = 0;
  eventStatuses = Object.entries(EventStatus)
    .filter(([key]) => isNaN(Number(key)))
    .map(([key, value]) => ({ name: key, value }));
  selectedFile: File | null = null;
  imagePreviewUrl: string | ArrayBuffer | null = null;
  
  constructor(
    private fb: FormBuilder,
    private eventService: EventService,
    private route: ActivatedRoute,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.subEventForm = this.createSubEventForm();
  }

  ngOnInit(): void {
    this.isLoading = true;
    
    // Get parent event ID from route
    const parentIdParam = this.route.snapshot.paramMap.get('parentId');
    if (!parentIdParam) {
      this.snackBar.open('Invalid parent event ID', 'Close', {
        duration: 5000,
        panelClass: 'error-snackbar'
      });
      this.router.navigate(['/events']);
      return;
    }
    
    this.parentId = parseInt(parentIdParam, 10);
    
    // Load parent event details
    this.eventService.getEventById(this.parentId).subscribe({
      next: (event) => {
        this.parentEvent = event;
        
        if (event.type !== EventType.Series) {
          this.snackBar.open('Parent event must be a Series event', 'Close', {
            duration: 5000,
            panelClass: 'error-snackbar'
          });
          this.router.navigate(['/events']);
          return;
        }
        
        // Prefill form with default values from parent
        this.initializeFormWithParentData(event);
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading parent event', error);
        this.snackBar.open('Error loading parent event. Please try again.', 'Close', {
          duration: 5000,
          panelClass: 'error-snackbar'
        });
        this.isLoading = false;
        this.router.navigate(['/events']);
      }
    });
  }

  createSubEventForm(): FormGroup {
    return this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', Validators.required],
      startDate: [new Date(), Validators.required],
      endDate: [new Date(), Validators.required],
      venueName: ['', Validators.required],
      capacity: [100, [Validators.required, Validators.min(1)]],
      status: [EventStatus.Draft, Validators.required],
      bannerImageUrl: ['']
    });
  }

  initializeFormWithParentData(parentEvent: EventModel): void {
    const startDate = new Date(parentEvent.startDate);
    // Default 2-hour sub-event
    const endDate = new Date(startDate);
    endDate.setHours(endDate.getHours() + 2);
    
    this.subEventForm.patchValue({
      venueName: parentEvent.venueName,
      capacity: parentEvent.capacity,
      startDate: startDate,
      endDate: endDate
    });
  }

  onSubmit(): void {
    if (this.subEventForm.invalid) {
      // Mark all fields as touched to trigger validation messages
      Object.keys(this.subEventForm.controls).forEach(key => {
        this.subEventForm.get(key)?.markAsTouched();
      });
      return;
    }
    
    this.isSubmitting = true;
    
    // Create sub-event data
    const subEventData = {
      ...this.subEventForm.value,
      type: EventType.SubEvent,
      parentEventId: this.parentId
    };
    
    // Handle file upload if there's a selected file
    if (this.selectedFile) {
      this.eventService.uploadImage(this.selectedFile).subscribe({
        next: (imageUrl) => {
          subEventData.bannerImageUrl = imageUrl;
          this.saveSubEvent(subEventData);
        },
        error: (error) => {
          console.error('Error uploading image', error);
          this.snackBar.open('Error uploading image. Saving sub-event without image.', 'Close', {
            duration: 5000
          });
          this.saveSubEvent(subEventData);
        }
      });
    } else {
      this.saveSubEvent(subEventData);
    }
  }

  saveSubEvent(subEventData: any): void {
    this.eventService.createEvent(subEventData).subscribe({
      next: (id) => {
        this.isSubmitting = false;
        this.snackBar.open('Sub-event created successfully', 'Close', {
          duration: 3000
        });
        this.router.navigate(['/events/edit', this.parentId]);
      },
      error: (error) => {
        console.error('Error creating sub-event', error);
        this.snackBar.open('Error creating sub-event. Please try again.', 'Close', {
          duration: 5000,
          panelClass: 'error-snackbar'
        });
        this.isSubmitting = false;
      }
    });
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
    this.subEventForm.patchValue({ bannerImageUrl: '' });
  }

  cancel(): void {
    this.router.navigate(['/events/edit', this.parentId]);
  }
}