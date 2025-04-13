import { Routes } from '@angular/router';
import { environment } from '@environments/environment';

import { DashboardComponent } from './features/dashboard/dashboard.component';
import { EventListComponent } from './features/events/event-list/event-list.component';
import { EventFormComponent } from './features/events/event-form/event-form.component';
import { SubEventFormComponent } from './features/events/sub-event-form/sub-event-form.component';

// Base routes that are always available
const baseRoutes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'events', component: EventListComponent },
  { path: 'events/new', component: EventFormComponent },
  { path: 'events/edit/:id', component: EventFormComponent },
];

// Feature-dependent routes
const subEventRoutes: Routes = environment.features.subEvents 
  ? [{ path: 'events/:parentId/sub-event/new', component: SubEventFormComponent }]
  : [];

// Combine all routes
export const routes: Routes = [
  ...baseRoutes,
  ...subEventRoutes,
  { path: '**', redirectTo: 'dashboard' } // Fallback route
];