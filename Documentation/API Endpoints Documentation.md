# Event Management Platform - API Endpoints Documentation

## Overview

The Event Management Platform exposes a RESTful API that follows standard HTTP conventions. All endpoints return JSON responses and accept JSON payloads where applicable. Authentication is handled via JWT tokens that should be included in the `Authorization` header with the `Bearer` prefix.

Base URL: `https://api.eventmanagement.example/v1`

## Authentication

### POST /auth/login

Authenticates a user and returns a JWT token.

**Request Body:**

```json
{
  "email": "user@example.com",
  "password": "securepassword"
}
```

**Response:**

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 3600,
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### POST /auth/register

Registers a new user.

**Request Body:**

```json
{
  "email": "newuser@example.com",
  "password": "securepassword",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+1234567890"
}
```

**Response:**

```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "email": "newuser@example.com"
}
```

### POST /auth/refresh

Refreshes an access token using a refresh token.

**Request Body:**

```json
{
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Response:**

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 3600,
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

## Events

### GET /events

Returns a paginated list of published events.

**Query Parameters:**

- `page` (default: 1): Page number
- `pageSize` (default: 10): Number of items per page
- `search`: Search term to filter by name or description
- `from`: Filter events starting from this date (ISO 8601 format)
- `to`: Filter events ending before this date (ISO 8601 format)

**Response:**

```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "Tech Conference 2025",
      "description": "Annual technology conference",
      "startDate": "2025-06-01T09:00:00Z",
      "endDate": "2025-06-03T17:00:00Z",
      "location": "Convention Center, New York",
      "type": "Series",
      "organizerId": "1fa85f64-5717-4562-b3fc-2c963f66afa7",
      "organizerName": "Tech Events Inc."
    }
  ],
  "totalCount": 45,
  "page": 1,
  "pageSize": 10,
  "totalPages": 5
}
```

### GET /events/{id}

Returns detailed information about a specific event.

**Response:**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Tech Conference 2025",
  "description": "Annual technology conference showcasing the latest innovations",
  "type": "Series",
  "startDate": "2025-06-01T09:00:00Z",
  "endDate": "2025-06-03T17:00:00Z",
  "location": "Convention Center, New York",
  "virtualMeetingUrl": "https://meeting.example/conf2025",
  "maxAttendees": 500,
  "isPublished": true,
  "status": "Published",
  "organizer": {
    "id": "1fa85f64-5717-4562-b3fc-2c963f66afa7",
    "name": "Tech Events Inc.",
    "description": "Leading technology event organizer",
    "logoUrl": "https://storage.example/logos/techevents.png",
    "website": "https://techevents.example"
  },
  "subEvents": [
    {
      "id": "4fa85f64-5717-4562-b3fc-2c963f66afa8",
      "name": "Opening Day",
      "startDate": "2025-06-01T09:00:00Z",
      "endDate": "2025-06-01T17:00:00Z"
    },
    {
      "id": "5fa85f64-5717-4562-b3fc-2c963f66afa9",
      "name": "Workshop Day",
      "startDate": "2025-06-02T09:00:00Z",
      "endDate": "2025-06-02T17:00:00Z"
    }
  ],
  "ticketTypes": [
    {
      "id": "6fa85f64-5717-4562-b3fc-2c963f66afb0",
      "name": "General Admission",
      "description": "Access to all general sessions",
      "price": 199.99,
      "availableQuantity": 400,
      "salesStartDate": "2025-01-01T00:00:00Z",
      "salesEndDate": "2025-05-15T23:59:59Z"
    },
    {
      "id": "7fa85f64-5717-4562-b3fc-2c963f66afb1",
      "name": "VIP Pass",
      "description": "Full access including workshops and networking events",
      "price": 499.99,
      "availableQuantity": 100,
      "salesStartDate": "2025-01-01T00:00:00Z",
      "salesEndDate": "2025-05-01T23:59:59Z"
    }
  ]
}
```

### POST /events

Creates a new event.

**Request Body:**

```json
{
  "name": "Product Launch",
  "description": "Launching our new product line",
  "type": "Standalone",
  "startDate": "2025-07-15T10:00:00Z",
  "endDate": "2025-07-15T16:00:00Z",
  "location": "Grand Hotel, Chicago",
  "virtualMeetingUrl": "https://meeting.example/productlaunch",
  "maxAttendees": 200
}
```

**Response:**

```json
{
  "id": "8fa85f64-5717-4562-b3fc-2c963f66afb2",
  "name": "Product Launch",
  "description": "Launching our new product line",
  "type": "Standalone",
  "startDate": "2025-07-15T10:00:00Z",
  "endDate": "2025-07-15T16:00:00Z",
  "location": "Grand Hotel, Chicago",
  "virtualMeetingUrl": "https://meeting.example/productlaunch",
  "maxAttendees": 200,
  "isPublished": false,
  "status": "Draft",
  "organizerId": "1fa85f64-5717-4562-b3fc-2c963f66afa7"
}
```

### PUT /events/{id}

Updates an existing event.

**Request Body:**

```json
{
  "name": "Product Launch - Updated",
  "description": "Launching our new product line with exclusive demos",
  "startDate": "2025-07-16T10:00:00Z",
  "endDate": "2025-07-16T16:00:00Z",
  "location": "Luxury Hotel, Chicago",
  "virtualMeetingUrl": "https://meeting.example/productlaunch2",
  "maxAttendees": 250
}
```

**Response:**

```json
{
  "id": "8fa85f64-5717-4562-b3fc-2c963f66afb2",
  "name": "Product Launch - Updated",
  "description": "Launching our new product line with exclusive demos",
  "type": "Standalone",
  "startDate": "2025-07-16T10:00:00Z",
  "endDate": "2025-07-16T16:00:00Z",
  "location": "Luxury Hotel, Chicago",
  "virtualMeetingUrl": "https://meeting.example/productlaunch2",
  "maxAttendees": 250,
  "isPublished": false,
  "status": "Draft",
  "organizerId": "1fa85f64-5717-4562-b3fc-2c963f66afa7"
}
```

### POST /events/{id}/publish

Publishes an event, making it visible to users.

**Response:**

```json
{
  "id": "8fa85f64-5717-4562-b3fc-2c963f66afb2",
  "name": "Product Launch - Updated",
  "isPublished": true,
  "status": "Published",
  "publishedDate": "2025-04-20T14:30:45Z"
}
```

### POST /events/{id}/cancel

Cancels a published event.

**Request Body:**

```json
{
  "reason": "Insufficient registrations"
}
```

**Response:**

```json
{
  "id": "8fa85f64-5717-4562-b3fc-2c963f66afb2",
  "name": "Product Launch - Updated",
  "status": "Cancelled",
  "cancellationReason": "Insufficient registrations",
  "cancelledDate": "2025-05-01T09:15:30Z"
}
```

### POST /events/{id}/subevents

Adds a sub-event to a series event.

**Request Body:**

```json
{
  "name": "Closing Ceremony",
  "description": "Final day wrap-up and awards",
  "startDate": "2025-06-03T15:00:00Z",
  "endDate": "2025-06-03T17:00:00Z",
  "location": "Main Hall, Convention Center"
}
```

**Response:**

```json
{
  "id": "9fa85f64-5717-4562-b3fc-2c963f66afb3",
  "name": "Closing Ceremony",
  "description": "Final day wrap-up and awards",
  "type": "Standalone",
  "startDate": "2025-06-03T15:00:00Z",
  "endDate": "2025-06-03T17:00:00Z",
  "location": "Main Hall, Convention Center",
  "parentEventId": "3fa85f64-5717-4562-b3fc-2c963f66afa
}
```

## Sessions

### GET /events/{eventId}/sessions

Returns all sessions for a specific event.

**Query Parameters:**

- `page` (default: 1): Page number
- `pageSize` (default: 10): Number of items per page
- `date`: Filter sessions by specific date (ISO 8601 format)

**Response:**

json

```json
{
  "items": [
    {
      "id": "4fa85f64-5717-4562-b3fc-2c963f66afc0",
      "title": "Keynote: Future of Technology",
      "description": "Opening keynote discussing emerging tech trends",
      "startTime": "2025-06-01T09:30:00Z",
      "endTime": "2025-06-01T10:30:00Z",
      "location": "Main Hall",
      "maxAttendees": 500,
      "currentAttendees": 320,
      "speakers": [
        {
          "id": "1fa85f64-5717-4562-b3fc-2c963f66afd1",
          "name": "Jane Smith",
          "photoUrl": "https://storage.example/speakers/janesmith.jpg"
        }
      ]
    },
    {
      "id": "5fa85f64-5717-4562-b3fc-2c963f66afc2",
      "title": "AI in Enterprise",
      "description": "How AI is transforming business operations",
      "startTime": "2025-06-01T11:00:00Z",
      "endTime": "2025-06-01T12:00:00Z",
      "location": "Room A",
      "maxAttendees": 150,
      "currentAttendees": 98,
      "speakers": [
        {
          "id": "2fa85f64-5717-4562-b3fc-2c963f66afd3",
          "name": "Robert Johnson",
          "photoUrl": "https://storage.example/speakers/robertjohnson.jpg"
        }
      ]
    }
  ],
  "totalCount": 24,
  "page": 1,
  "pageSize": 10,
  "totalPages": 3
}
```

### GET /events/{eventId}/sessions/{id}

Returns detailed information about a specific session.

**Response:**

json

```json
{
  "id": "4fa85f64-5717-4562-b3fc-2c963f66afc0",
  "title": "Keynote: Future of Technology",
  "description": "Opening keynote discussing emerging tech trends and their impact on industry over the next decade. This session will cover AI, quantum computing, and sustainable technology.",
  "startTime": "2025-06-01T09:30:00Z",
  "endTime": "2025-06-01T10:30:00Z",
  "location": "Main Hall",
  "virtualMeetingUrl": "https://meeting.example/tech-keynote",
  "maxAttendees": 500,
  "currentAttendees": 320,
  "speakers": [
    {
      "id": "1fa85f64-5717-4562-b3fc-2c963f66afd1",
      "name": "Jane Smith",
      "bio": "CEO of TechInnovate, with 20+ years experience in the industry",
      "photoUrl": "https://storage.example/speakers/janesmith.jpg",
      "topics": "AI, Innovation, Leadership"
    }
  ],
  "materials": [
    {
      "id": "1fa85f64-5717-4562-b3fc-2c963f66afe1",
      "name": "Keynote Slides",
      "fileUrl": "https://storage.example/materials/keynote-slides.pdf",
      "fileType": "application/pdf"
    }
  ]
}
```

### POST /events/{eventId}/sessions

Creates a new session for an event.

**Request Body:**

json

```json
{
  "title": "Workshop: Cloud Deployment Strategies",
  "description": "Hands-on workshop covering modern cloud deployment techniques",
  "startTime": "2025-06-02T14:00:00Z",
  "endTime": "2025-06-02T16:00:00Z",
  "location": "Workshop Room B",
  "virtualMeetingUrl": "https://meeting.example/cloud-workshop",
  "maxAttendees": 50,
  "speakerIds": ["3fa85f64-5717-4562-b3fc-2c963f66afd4"]
}
```

**Response:**

json

```json
{
  "id": "6fa85f64-5717-4562-b3fc-2c963f66afc5",
  "title": "Workshop: Cloud Deployment Strategies",
  "description": "Hands-on workshop covering modern cloud deployment techniques",
  "startTime": "2025-06-02T14:00:00Z",
  "endTime": "2025-06-02T16:00:00Z",
  "location": "Workshop Room B",
  "virtualMeetingUrl": "https://meeting.example/cloud-workshop",
  "maxAttendees": 50,
  "currentAttendees": 0,
  "speakers": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afd4",
      "name": "Michael Chen",
      "photoUrl": "https://storage.example/speakers/michaelchen.jpg"
    }
  ]
}
```

### PUT /events/{eventId}/sessions/{id}

Updates an existing session.

**Request Body:**

json

```json
{
  "title": "Extended Workshop: Advanced Cloud Strategies",
  "description": "Extended hands-on workshop covering advanced cloud deployment techniques",
  "startTime": "2025-06-02T13:30:00Z",
  "endTime": "2025-06-02T16:30:00Z",
  "location": "Workshop Room A",
  "maxAttendees": 40
}
```

**Response:**

json

```json
{
  "id": "6fa85f64-5717-4562-b3fc-2c963f66afc5",
  "title": "Extended Workshop: Advanced Cloud Strategies",
  "description": "Extended hands-on workshop covering advanced cloud deployment techniques",
  "startTime": "2025-06-02T13:30:00Z",
  "endTime": "2025-06-02T16:30:00Z",
  "location": "Workshop Room A",
  "virtualMeetingUrl": "https://meeting.example/cloud-workshop",
  "maxAttendees": 40,
  "currentAttendees": 0,
  "speakers": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afd4",
      "name": "Michael Chen",
      "photoUrl": "https://storage.example/speakers/michaelchen.jpg"
    }
  ]
}
```

### DELETE /events/{eventId}/sessions/{id}

Deletes a session.

**Response Status:** 204 No Content

### POST /events/{eventId}/sessions/{id}/speakers/{speakerId}

Assigns a speaker to a session.

**Response Status:** 204 No Content

### DELETE /events/{eventId}/sessions/{id}/speakers/{speakerId}

Removes a speaker from a session.

**Response Status:** 204 No Content

## Ticket Types

### GET /events/{eventId}/tickettypes

Returns all ticket types for a specific event.

**Response:**

json

```json
{
  "items": [
    {
      "id": "6fa85f64-5717-4562-b3fc-2c963f66afb0",
      "name": "General Admission",
      "description": "Access to all general sessions",
      "price": 199.99,
      "availableQuantity": 400,
      "remainingQuantity": 237,
      "salesStartDate": "2025-01-01T00:00:00Z",
      "salesEndDate": "2025-05-15T23:59:59Z",
      "scope": "SingleEvent"
    },
    {
      "id": "7fa85f64-5717-4562-b3fc-2c963f66afb1",
      "name": "VIP Pass",
      "description": "Full access including workshops and networking events",
      "price": 499.99,
      "availableQuantity": 100,
      "remainingQuantity": 52,
      "salesStartDate": "2025-01-01T00:00:00Z",
      "salesEndDate": "2025-05-01T23:59:59Z",
      "scope": "AllAccess"
    }
  ]
}
```

### POST /events/{eventId}/tickettypes

Creates a new ticket type for an event.

**Request Body:**

json

```json
{
  "name": "Early Bird Special",
  "description": "Discounted early bird tickets",
  "price": 149.99,
  "availableQuantity": 100,
  "salesStartDate": "2025-01-01T00:00:00Z",
  "salesEndDate": "2025-02-28T23:59:59Z",
  "scope": "SingleEvent"
}
```

**Response:**

json

```json
{
  "id": "8fa85f64-5717-4562-b3fc-2c963f66afb7",
  "name": "Early Bird Special",
  "description": "Discounted early bird tickets",
  "price": 149.99,
  "availableQuantity": 100,
  "remainingQuantity": 100,
  "salesStartDate": "2025-01-01T00:00:00Z",
  "salesEndDate": "2025-02-28T23:59:59Z",
  "scope": "SingleEvent"
}
```

### PUT /events/{eventId}/tickettypes/{id}

Updates an existing ticket type.

**Request Body:**

json

```json
{
  "name": "Early Bird Special",
  "description": "Limited time offer: discounted early bird tickets",
  "price": 129.99,
  "availableQuantity": 150,
  "salesEndDate": "2025-03-15T23:59:59Z"
}
```

**Response:**

json

```json
{
  "id": "8fa85f64-5717-4562-b3fc-2c963f66afb7",
  "name": "Early Bird Special",
  "description": "Limited time offer: discounted early bird tickets",
  "price": 129.99,
  "availableQuantity": 150,
  "remainingQuantity": 150,
  "salesStartDate": "2025-01-01T00:00:00Z",
  "salesEndDate": "2025-03-15T23:59:59Z",
  "scope": "SingleEvent"
}
```

### DELETE /events/{eventId}/tickettypes/{id}

Deletes a ticket type.

**Response Status:** 204 No Content

## Registrations

### GET /events/{eventId}/registrations

Returns all registrations for an event (organizer only).

**Query Parameters:**

- `page` (default: 1): Page number
- `pageSize` (default: 10): Number of items per page
- `status`: Filter by status (Pending, Confirmed, CheckedIn, Cancelled)

**Response:**

json

```json
{
  "items": [
    {
      "id": "9fa85f64-5717-4562-b3fc-2c963f66afc7",
      "registrationDate": "2025-02-15T14:23:15Z",
      "status": "Confirmed",
      "user": {
        "id": "1fa85f64-5717-4562-b3fc-2c963f66afd8",
        "email": "attendee1@example.com",
        "firstName": "John",
        "lastName": "Doe"
      },
      "tickets": [
        {
          "id": "1fa85f64-5717-4562-b3fc-2c963f66afe2",
          "ticketNumber": "CONF2025-001",
          "ticketType": "VIP Pass",
          "purchasePrice": 499.99,
          "isUsed": false
        }
      ]
    }
  ],
  "totalCount": 163,
  "page": 1,
  "pageSize": 10,
  "totalPages": 17
}
```

### GET /registrations/{id}

Returns detailed information about a specific registration.

**Response:**

json

```json
{
  "id": "9fa85f64-5717-4562-b3fc-2c963f66afc7",
  "registrationDate": "2025-02-15T14:23:15Z",
  "status": "Confirmed",
  "notes": "Special dietary requirements: vegetarian",
  "event": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Tech Conference 2025",
    "startDate": "2025-06-01T09:00:00Z",
    "endDate": "2025-06-03T17:00:00Z"
  },
  "user": {
    "id": "1fa85f64-5717-4562-b3fc-2c963f66afd8",
    "email": "attendee1@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "phoneNumber": "+1234567890"
  },
  "tickets": [
    {
      "id": "1fa85f64-5717-4562-b3fc-2c963f66afe2",
      "ticketNumber": "CONF2025-001",
      "ticketType": "VIP Pass",
      "purchasePrice": 499.99,
      "purchaseDate": "2025-02-15T14:23:15Z",
      "isUsed": false
    }
  ]
}
```

### POST /events/{eventId}/register

Registers the current user for an event.

**Request Body:**

json

```json
{
  "ticketTypeId": "7fa85f64-5717-4562-b3fc-2c963f66afb1",
  "notes": "Special dietary requirements: vegetarian"
}
```

**Response:**

json

```json
{
  "id": "9fa85f64-5717-4562-b3fc-2c963f66afc9",
  "registrationDate": "2025-04-20T10:45:22Z",
  "status": "Pending",
  "notes": "Special dietary requirements: vegetarian",
  "ticket": {
    "id": "2fa85f64-5717-4562-b3fc-2c963f66afe5",
    "ticketNumber": "CONF2025-164",
    "ticketType": "VIP Pass",
    "purchasePrice": 499.99
  },
  "paymentLink": "https://payment.example/checkout/9fa85f64"
}
```

### POST /registrations/{id}/confirm

Confirms a registration after payment.

**Response:**

json

```json
{
  "id": "9fa85f64-5717-4562-b3fc-2c963f66afc9",
  "status": "Confirmed",
  "confirmationDate": "2025-04-20T10:50:37Z",
  "confirmationCode": "CONF2025-C164"
}
```

### POST /registrations/{id}/checkin

Checks in an attendee at the event.

**Response:**

json

```json
{
  "id": "9fa85f64-5717-4562-b3fc-2c963f66afc9",
  "status": "CheckedIn",
  "checkInDate": "2025-06-01T08:45:12Z",
  "badgePrintUrl": "https://api.eventmanagement.example/v1/registrations/9fa85f64-5717-4562-b3fc-2c963f66afc9/badge"
}
```

### POST /registrations/{id}/cancel

Cancels a registration.

**Request Body:**

json

```json
{
  "reason": "Unable to attend due to scheduling conflict"
}
```

**Response:**

json

```json
{
  "id": "9fa85f64-5717-4562-b3fc-2c963f66afc9",
  "status": "Cancelled",
  "cancellationDate": "2025-05-15T16:32:45Z",
  "refundStatus": "Processing"
}
```

## Speakers

### GET /events/{eventId}/speakers

Returns all speakers for an event.

**Response:**

json

```json
{
  "items": [
    {
      "id": "1fa85f64-5717-4562-b3fc-2c963f66afd1",
      "userId": "5fa85f64-5717-4562-b3fc-2c963f66afa2",
      "name": "Jane Smith",
      "bio": "CEO of TechInnovate, with 20+ years experience in the industry",
      "photoUrl": "https://storage.example/speakers/janesmith.jpg",
      "topics": "AI, Innovation, Leadership",
      "sessions": [
        {
          "id": "4fa85f64-5717-4562-b3fc-2c963f66afc0",
          "title": "Keynote: Future of Technology",
          "startTime": "2025-06-01T09:30:00Z"
        }
      ]
    }
  ]
}
```

### POST /events/{eventId}/speakers

Adds a speaker to an event.

**Request Body:**

json

```json
{
  "userId": "6fa85f64-5717-4562-b3fc-2c963f66afa3",
  "bio": "Chief Architect at CloudTech Solutions",
  "topics": "Cloud Architecture, Microservices, DevOps",
  "photoUrl": "https://storage.example/speakers/lisajohnson.jpg"
}
```

**Response:**

json

```json
{
  "id": "4fa85f64-5717-4562-b3fc-2c963f66afd5",
  "userId": "6fa85f64-5717-4562-b3fc-2c963f66afa3",
  "name": "Lisa Johnson",
  "bio": "Chief Architect at CloudTech Solutions",
  "photoUrl": "https://storage.example/speakers/lisajohnson.jpg",
  "topics": "Cloud Architecture, Microservices, DevOps"
}
```

### PUT /speakers/{id}

Updates a speaker profile.

**Request Body:**

json

```json
{
  "bio": "Chief Architect and VP of Engineering at CloudTech Solutions",
  "topics": "Cloud Architecture, Microservices, DevOps, Serverless",
  "photoUrl": "https://storage.example/speakers/lisajohnson_new.jpg"
}
```

**Response:**

json

```json
{
  "id": "4fa85f64-5717-4562-b3fc-2c963f66afd5",
  "userId": "6fa85f64-5717-4562-b3fc-2c963f66afa3",
  "name": "Lisa Johnson",
  "bio": "Chief Architect and VP of Engineering at CloudTech Solutions",
  "photoUrl": "https://storage.example/speakers/lisajohnson_new.jpg",
  "topics": "Cloud Architecture, Microservices, DevOps, Serverless"
}
```

## Users

### GET /users/me

Returns the current user's profile.

**Response:**

json

```json
{
  "id": "1fa85f64-5717-4562-b3fc-2c963f66afd8",
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+1234567890",
  "emailConfirmed": true,
  "phoneConfirmed": true,
  "roles": ["Attendee", "Speaker"],
  "speakerProfile": {
    "id": "5fa85f64-5717-4562-b3fc-2c963f66afd7",
    "bio": "Software engineer specializing in AI applications",
    "photoUrl": "https://storage.example/speakers/johndoe.jpg",
    "topics": "AI, Machine Learning, Python"
  }
}
```

### PUT /users/me

Updates the current user's profile.

**Request Body:**

json

```json
{
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+1987654321"
}
```

**Response:**

json

```json
{
  "id": "1fa85f64-5717-4562-b3fc-2c963f66afd8",
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+1987654321",
  "emailConfirmed": true,
  "phoneConfirmed": false
}
```

### GET /users/me/registrations

Returns all registrations for the current user.

**Response:**

json

```json
{
  "items": [
    {
      "id": "9fa85f64-5717-4562-b3fc-2c963f66afc7",
      "status": "Confirmed",
      "registrationDate": "2025-02-15T14:23:15Z",
      "event": {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "Tech Conference 2025",
        "startDate": "2025-06-01T09:00:00Z",
        "endDate": "2025-06-03T17:00:00Z"
      },
      "ticket": {
        "ticketNumber": "CONF2025-001",
        "ticketType": "VIP Pass"
      }
    }
  ]
}
```

## Error Handling

All API endpoints follow a consistent error response format:

json

```json
{
  "statusCode": 400,
  "message": "Validation error",
  "errors": [
    {
      "field": "endDate",
      "message": "End date must be after start date"
    }
  ],
  "timestamp": "2025-04-20T12:34:56Z",
  "path": "/events",
  "requestId": "a1b2c3d4-e5f6-g7h8-i9j0-k1l2m3n4o5p6"
}
```

Common HTTP status codes:

- 200: OK - The request succeeded
- 201: Created - A new resource was created
- 204: No Content - The request succeeded, but there is no content to return
- 400: Bad Request - Invalid request parameters
- 401: Unauthorized - Authentication required
- 403: Forbidden - Insufficient permissions
- 404: Not Found - Resource not found
- 409: Conflict - Request conflicts with current state
- 422: Unprocessable Entity - Validation error
- 500: Internal Server Error - Server error

## Pagination

All collection endpoints support pagination with the following parameters:

- `page`: The page number (1-indexed)
- `pageSize`: Number of items per page (default 10, max 100)

And return pagination metadata:

json

```json
{
  "items": [...],
  "totalCount": 45,
  "page": 1,
  "pageSize": 10,
  "totalPages": 5
}
```

## Filtering and Sorting

Most collection endpoints support filtering through query parameters specific to the resource type. Common filtering options include:

- `search`: Free text search
- `from`/`to`: Date range filters
- `status`: Filter by status

Sorting is supported via:

- `sortBy`: Field to sort by
- `sortOrder`: "asc" or "desc" (default "asc")

Example: `/events?sortBy=startDate&sortOrder=desc`