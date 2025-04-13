export enum EventType {
    Standalone = 0,
    Series = 1,
    SubEvent = 2
  }
  
  export enum EventStatus {
    Draft = 0,
    Published = 1,
    Canceled = 2,
    Completed = 3
  }
  
  export interface Event {
    id: number;
    title: string;
    description: string;
    startDate: string;
    endDate: string;
    venueName: string;
    capacity: number;
    type: EventType;
    parentEventId?: number;
    parentEvent?: Event;
    status: EventStatus;
    bannerImageUrl?: string;
    subEvents?: Event[];
  }
  
  export interface EventFilter {
    searchTerm?: string;
    eventType?: EventType;
    status?: EventStatus;
  }