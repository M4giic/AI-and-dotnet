import { Component, OnInit } from '@angular/core';
import { EventService }  from '@app/core/services/event.service'
import { Event as EventModel, EventStatus, EventType } from '@app/core/models/event.model';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

import { environment } from '@environments/environment';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'], 
  imports:[
    CommonModule,
    MatProgressSpinnerModule,
    MatCardModule,
    MatIconModule,
    MatListModule,
    RouterLink
  ]
})
export class DashboardComponent implements OnInit {
  subEventsEnabled = environment.features.subEvents;

  EventType = EventType;
  EventStatus = EventStatus;
  
  events: EventModel[] = [];
  recentEvents: EventModel[] = [];
  upcomingEvents: EventModel[] = [];
  isLoading = true;
  
  // Stats
  totalEvents = 0;
  publishedEvents = 0;
  draftEvents = 0;
  upcomingEventsCount = 0;

  constructor(private eventService: EventService) { }

  ngOnInit(): void {
    this.loadEvents();
  }

  loadEvents(): void {
    this.isLoading = true;
    this.eventService.getAllEvents().subscribe({
      next: (events) => {
        this.events = events;
        this.calculateStats();
        
        // Get recent events (last 5 added)
        this.recentEvents = [...this.events]
          .sort((a, b) => b.id - a.id)
          .slice(0, 5);
          
        // Get upcoming events (next 5)
        const today = new Date();
        this.upcomingEvents = [...this.events]
          .filter(e => new Date(e.startDate) > today)
          .sort((a, b) => new Date(a.startDate).getTime() - new Date(b.startDate).getTime())
          .slice(0, 5);
          
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading events', error);
        this.isLoading = false;
      }
    });
  }

  calculateStats(): void {
    this.totalEvents = this.events.length;
    
    const today = new Date();
    
    this.publishedEvents = this.events.filter(e => 
      e.status === EventStatus.Published
    ).length;
    
    this.draftEvents = this.events.filter(e => 
      e.status === EventStatus.Draft
    ).length;
    
    this.upcomingEventsCount = this.events.filter(e => 
      new Date(e.startDate) > today && 
      (e.status === EventStatus.Published || e.status === EventStatus.Draft)
    ).length;
  }

  getEventStatusClass(status: EventStatus): string {
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

  getEventStatusText(status: EventStatus): string {
    return EventStatus[status];
  }
}