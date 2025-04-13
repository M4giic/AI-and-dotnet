import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { EventService } from 'src/app/core/services/event.service';
import { Event, EventStatus, EventType, EventFilter } from 'src/app/core/models/event.model';
import { ConfirmDialogComponent } from 'src/app/shared/components/confirm-dialog/confirm-dialog.component';
import { environment } from '@environments/environment';


@Component({
  selector: 'app-event-list',
  templateUrl: './event-list.component.html',
  styleUrls: ['./event-list.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    FormsModule,
    ReactiveFormsModule,
    MatTableModule,
    MatSortModule,
    MatPaginatorModule,
    MatInputModule,
    MatSelectModule,
    MatFormFieldModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    MatSnackBarModule
  ]
})
export class EventListComponent implements OnInit {
  subEventsEnabled = environment.features.subEvents;

  displayedColumns: string[] = ['title', 'type', 'dates', 'venue', 'status', 'actions'];
  dataSource: MatTableDataSource<Event>;
  events: Event[] = [];
  isLoading = true;
  
  searchTerm = new FormControl('');
  eventTypeFilter = new FormControl<number | null>(null);
  statusFilter = new FormControl<number | null>(null);
  
  eventTypes = Object.entries(EventType)
    .filter(([key]) => isNaN(Number(key)))
    .map(([key, value]) => ({ name: key, value }));
  
  eventStatuses = Object.entries(EventStatus)
    .filter(([key]) => isNaN(Number(key)))
    .map(([key, value]) => ({ name: key, value }));
  
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private eventService: EventService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {
    this.dataSource = new MatTableDataSource();
  }

  ngOnInit(): void {
    this.loadEvents();
  }

  ngAfterViewInit() {
    if (this.dataSource) {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    }
  }

  loadEvents(): void {
    this.isLoading = true;
    this.eventService.getAllEvents().subscribe({
      next: (events) => {
        this.events = events;
        this.dataSource.data = this.events;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading events', error);
        this.snackBar.open('Error loading events. Please try again.', 'Close', {
          duration: 5000,
          panelClass: 'error-snackbar'
        });
        this.isLoading = false;
      }
    });
  }

  applyFilter(): void {
    const filterValue = {
      searchTerm: this.searchTerm.value || '',
      eventType: this.eventTypeFilter.value !== null ? Number(this.eventTypeFilter.value) : undefined,
      status: this.statusFilter.value !== null ? Number(this.statusFilter.value) : undefined
    } as EventFilter;
    
    this.dataSource.data = this.filterEvents(this.events, filterValue);
    
    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  filterEvents(events: Event[], filter: EventFilter): Event[] {
    return events.filter(event => {
      // Apply search term filter
      const matchesSearchTerm = !filter.searchTerm || 
        event.title.toLowerCase().includes(filter.searchTerm.toLowerCase()) ||
        event.description.toLowerCase().includes(filter.searchTerm.toLowerCase());
      
      // Apply event type filter
      const matchesEventType = filter.eventType === undefined || 
        event.type === filter.eventType;
      
      // Apply status filter
      const matchesStatus = filter.status === undefined || 
        event.status === filter.status;
      
      return matchesSearchTerm && matchesEventType && matchesStatus;
    });
  }

  resetFilters(): void {
    this.searchTerm.setValue('');
    this.eventTypeFilter.setValue(null);
    this.statusFilter.setValue(null);
    this.dataSource.data = this.events;
  }

  deleteEvent(id: number): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '350px',
      data: {
        title: 'Confirm Delete',
        message: 'Are you sure you want to delete this event? This action cannot be undone.',
        confirmButtonText: 'Delete',
        cancelButtonText: 'Cancel'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.eventService.deleteEvent(id).subscribe({
          next: () => {
            this.loadEvents();
            this.snackBar.open('Event deleted successfully', 'Close', {
              duration: 3000
            });
          },
          error: (error) => {
            console.error('Error deleting event', error);
            this.snackBar.open('Error deleting event. Please try again.', 'Close', {
              duration: 5000,
              panelClass: 'error-snackbar'
            });
          }
        });
      }
    });
  }

  getEventTypeName(type: EventType): string {
    return EventType[type];
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

  getEventStatusName(status: EventStatus): string {
    return EventStatus[status];
  }
}