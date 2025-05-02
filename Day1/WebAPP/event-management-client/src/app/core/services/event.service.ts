import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { delay, map } from 'rxjs/operators';
import { Event as EventModel, EventType } from '@core/models/event.model';
import { environment } from '@environments/environment';

@Injectable({
  providedIn: 'root'
})
export class EventService {
  private apiUrl = `${environment.apiUrl}/api/events`;

  constructor(private http: HttpClient) { }

  getAllEvents(): Observable<EventModel[]> {
    return this.http.get<EventModel[]>(this.apiUrl);
  }

  getEventById(id: number): Observable<EventModel> {
    return this.http.get<EventModel>(`${this.apiUrl}/${id}`);
  }

  getEventsByType(type: EventType): Observable<EventModel[]> {
    // Check if the feature is enabled
    if (environment.features.eventsByType) {
      return this.http.get<EventModel[]>(`${this.apiUrl}/bytype/${type}`);
    } else {
      // Fallback: filter events from the main endpoint
      return this.getAllEvents().pipe(
        map(events => events.filter(event => event.type === type))
      );
    }
  }

  getSubEvents(parentId: number): Observable<EventModel[]> {
    // Check if the feature is enabled
    if (environment.features.subEvents) {
      return this.http.get<EventModel[]>(`${this.apiUrl}/${parentId}/subevents`);
    } else {
      // Fallback: filter events from the main endpoint
      return this.getAllEvents().pipe(
        map(events => events.filter(event => 
          event.type === EventType.SubEvent && event.parentEventId === parentId
        ))
      );
    }
  }

  createEvent(event: EventModel): Observable<number> {
    return this.http.post<number>(this.apiUrl, event);
  }

  updateEvent(event: EventModel): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${event.id}`, event);
  }

  deleteEvent(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  uploadImage(file: File): Observable<string> {
    // Check if we have an upload endpoint configured
    if (environment.features.fileUpload) {
      // Creates a FormData object to send the file
      const formData = new FormData();
      formData.append('file', file);
      
      // API endpoint for image upload
      return this.http.post<string>(`${environment.apiUrl}/api/upload`, formData);
    } else {
      // Mock implementation - generate a placeholder URL
      console.log('Using mock image upload. File would have been:', file.name);
      
      // Return a mock URL after a slight delay to simulate network request
      return of(`/assets/images/placeholders/event-${Math.floor(Math.random() * 5) + 1}.jpg`)
        .pipe(delay(800));  // simulate network delay
    }
  }
}