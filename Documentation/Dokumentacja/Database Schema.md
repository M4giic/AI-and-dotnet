# Event Management Platform - Database Schema

## Overview

The Event Management Platform uses SQL Server as its primary data store. The database schema is designed to support the domain model while optimizing for query performance and data integrity.

## Entity Relationship Diagram

```
┌───────────────┐       ┌───────────────┐       ┌───────────────┐
│     Users     │       │   Organizers  │       │    Events     │
├───────────────┤       ├───────────────┤       ├───────────────┤
│ Id            │───┐   │ Id            │───┐   │ Id            │
│ Email         │   │   │ UserId        │   │   │ Name          │
│ PasswordHash  │   │   │ OrgName       │   │   │ Description   │
│ FirstName     │   └──>│ Description   │   │   │ Type          │
│ LastName      │       │ LogoUrl       │   │   │ StartDate     │
│ PhoneNumber   │       │ Website       │   │   │ EndDate       │
│ EmailConfirmed│       └───────────────┘   │   │ Location      │
│ PhoneConfirmed│                           │   │ VirtualMeetUrl│
└───────────────┘                           │   │ MaxAttendees  │
        │                                   │   │ IsPublished   │
        │                                   └──>│ OrganizerId   │
        │                                       │ Status        │
        │                                       │ ParentEventId │
┌───────┴───────┐                               └───────────────┘
│  UserRoles    │                                      │
├───────────────┤                                      │
│ UserId        │                                      │
│ Role          │                                      │
└───────────────┘                                      │
                                                       │
┌───────────────┐                               ┌──────┴───────┐
│    Speakers   │                               │   Sessions   │
├───────────────┤                               ├──────────────┤
│ Id            │                               │ Id           │
│ UserId        │                               │ EventId      │
│ Bio           │◄──────────────────────────────│ Title        │
│ PhotoUrl      │                               │ Description  │
│ Topics        │                               │ StartTime    │
└───────────────┘                               │ EndTime      │
        ▲                                       │ MaxAttendees │
        │                                       │ Location     │
┌───────┴───────┐                               │ VirtualUrl   │
│SessionSpeakers│                               └──────────────┘
├───────────────┤                                      │
│ SessionId     │                                      │
│ SpeakerId     │                                      │
└───────────────┘                                      │
                                                       │
┌───────────────┐       ┌───────────────┐       ┌──────┴───────┐
│RegistrationSes│       │ Registrations │       │ TicketTypes  │
├───────────────┤       ├───────────────┤       ├──────────────┤
│ RegistrationId│◄──────│ Id            │       │ Id           │
│ SessionId     │       │ EventId       │       │ EventId      │
└───────────────┘       │ UserId        │       │ Name         │
                        │ RegDate       │       │ Description  │
                        │ Status        │       │ Price        │
                        │ Notes         │       │ AvailableQty │
                        └───────────────┘       │ SalesStartDt │
                               │                │ SalesEndDate │
                               │                │ Scope        │
                        ┌──────┴────────┐       └──────────────┘
                        │    Tickets    │              │
                        ├───────────────┤              │
                        │ Id            │              │
                        │ RegistrationId│              │
                        │ TicketTypeId  │◄─────────────┘
                        │ TicketNumber  │
                        │ PurchasePrice │
                        │ PurchaseDate  │
                        │ IsUsed        │
                        │ UsedDate      │
                        └───────────────┘
```

## Tables

### Users

Stores user account information.

```sql
CREATE TABLE [dbo].[Users] (
    [Id]                UNIQUEIDENTIFIER    NOT NULL PRIMARY KEY DEFAULT (NEWID()),
    [Email]             NVARCHAR(255)       NOT NULL UNIQUE,
    [PasswordHash]      NVARCHAR(MAX)       NOT NULL,
    [FirstName]         NVARCHAR(100)       NOT NULL,
    [LastName]          NVARCHAR(100)       NOT NULL,
    [PhoneNumber]       NVARCHAR(20)        NULL,
    [EmailConfirmed]    BIT                 NOT NULL DEFAULT(0),
    [PhoneConfirmed]    BIT                 NOT NULL DEFAULT(0),
    [CreatedAt]         DATETIME2           NOT NULL DEFAULT(GETUTCDATE()),
    [UpdatedAt]         DATETIME2           NOT NULL DEFAULT(GETUTCDATE())
);

-- Index on Email
CREATE NONCLUSTERED INDEX [IX_Users_Email] ON [dbo].[Users] ([Email]);
```

### UserRoles

Maps users to their assigned roles.

```sql
CREATE TABLE [dbo].[UserRoles] (
    [UserId]    UNIQUEIDENTIFIER    NOT NULL,
    [Role]      INT                 NOT NULL,  -- 1=Admin, 2=Organizer, 3=Speaker, 4=Attendee
    PRIMARY KEY ([UserId], [Role]),
    CONSTRAINT [FK_UserRoles_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
);

-- Index on Role
CREATE NONCLUSTERED INDEX [IX_UserRoles_Role] ON [dbo].[UserRoles] ([Role]);
```

### Organizers

Contains organizer-specific profile information.

```sql
CREATE TABLE [dbo].[Organizers] (
    [Id]                UNIQUEIDENTIFIER    NOT NULL PRIMARY KEY DEFAULT (NEWID()),
    [UserId]            UNIQUEIDENTIFIER    NOT NULL UNIQUE,
    [OrganizationName]  NVARCHAR(255)       NOT NULL,
    [Description]       NVARCHAR(MAX)       NULL,
    [LogoUrl]           NVARCHAR(255)       NULL,
    [Website]           NVARCHAR(255)       NULL,
    [CreatedAt]         DATETIME2           NOT NULL DEFAULT(GETUTCDATE()),
    [UpdatedAt]         DATETIME2           NOT NULL DEFAULT(GETUTCDATE()),
    CONSTRAINT [FK_Organizers_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
);
```

### Speakers

sql

```sql
CREATE TABLE [dbo].[Speakers] (
    [Id]                UNIQUEIDENTIFIER    NOT NULL PRIMARY KEY DEFAULT (NEWID()),
    [UserId]            UNIQUEIDENTIFIER    NOT NULL UNIQUE,
    [Bio]               NVARCHAR(MAX)       NULL,
    [PhotoUrl]          NVARCHAR(255)       NULL,
    [Topics]            NVARCHAR(255)       NULL,
    [CreatedAt]         DATETIME2           NOT NULL DEFAULT(GETUTCDATE()),
    [UpdatedAt]         DATETIME2           NOT NULL DEFAULT(GETUTCDATE()),
    CONSTRAINT [FK_Speakers_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
);
```

### Events

Stores event information, supporting both standalone and series events.

sql

```sql
CREATE TABLE [dbo].[Events] (
    [Id]                UNIQUEIDENTIFIER    NOT NULL PRIMARY KEY DEFAULT (NEWID()),
    [Name]              NVARCHAR(255)       NOT NULL,
    [Description]       NVARCHAR(MAX)       NULL,
    [Type]              INT                 NOT NULL,  -- 1=Standalone, 2=Series
    [StartDate]         DATETIME2           NULL,
    [EndDate]           DATETIME2           NULL,
    [Location]          NVARCHAR(255)       NULL,
    [VirtualMeetingUrl] NVARCHAR(255)       NULL,
    [MaxAttendees]      INT                 NOT NULL DEFAULT(0),  -- 0 means unlimited
    [IsPublished]       BIT                 NOT NULL DEFAULT(0),
    [OrganizerId]       UNIQUEIDENTIFIER    NOT NULL,
    [Status]            INT                 NOT NULL DEFAULT(1),  -- 1=Draft, 2=Published, 3=Cancelled, 4=Completed
    [ParentEventId]     UNIQUEIDENTIFIER    NULL,
    [CreatedAt]         DATETIME2           NOT NULL DEFAULT(GETUTCDATE()),
    [UpdatedAt]         DATETIME2           NOT NULL DEFAULT(GETUTCDATE()),
    CONSTRAINT [FK_Events_Organizers] FOREIGN KEY ([OrganizerId]) REFERENCES [dbo].[Organizers] ([Id]),
    CONSTRAINT [FK_Events_Events] FOREIGN KEY ([ParentEventId]) REFERENCES [dbo].[Events] ([Id])
);

-- Indexes
CREATE NONCLUSTERED INDEX [IX_Events_OrganizerId] ON [dbo].[Events] ([OrganizerId]);
CREATE NONCLUSTERED INDEX [IX_Events_ParentEventId] ON [dbo].[Events] ([ParentEventId]);
CREATE NONCLUSTERED INDEX [IX_Events_StartDate] ON [dbo].[Events] ([StartDate]);
CREATE NONCLUSTERED INDEX [IX_Events_Status_IsPublished] ON [dbo].[Events] ([Status], [IsPublished]);
```

### Sessions

Stores session information for events.

sql

```sql
CREATE TABLE [dbo].[Sessions] (
    [Id]                UNIQUEIDENTIFIER    NOT NULL PRIMARY KEY DEFAULT (NEWID()),
    [EventId]           UNIQUEIDENTIFIER    NOT NULL,
    [Title]             NVARCHAR(255)       NOT NULL,
    [Description]       NVARCHAR(MAX)       NULL,
    [StartTime]         DATETIME2           NOT NULL,
    [EndTime]           DATETIME2           NOT NULL,
    [MaxAttendees]      INT                 NOT NULL DEFAULT(0),  -- 0 means unlimited
    [Location]          NVARCHAR(255)       NULL,
    [VirtualMeetingUrl] NVARCHAR(255)       NULL,
    [CreatedAt]         DATETIME2           NOT NULL DEFAULT(GETUTCDATE()),
    [UpdatedAt]         DATETIME2           NOT NULL DEFAULT(GETUTCDATE()),
    CONSTRAINT [FK_Sessions_Events] FOREIGN KEY ([EventId]) REFERENCES [dbo].[Events] ([Id]) ON DELETE CASCADE
);

-- Indexes
CREATE NONCLUSTERED INDEX [IX_Sessions_EventId] ON [dbo].[Sessions] ([EventId]);
CREATE NONCLUSTERED INDEX [IX_Sessions_StartTime] ON [dbo].[Sessions] ([StartTime]);
```

### SessionSpeakers

Maps sessions to their speakers (many-to-many).

sql

```sql
CREATE TABLE [dbo].[SessionSpeakers] (
    [SessionId]         UNIQUEIDENTIFIER    NOT NULL,
    [SpeakerId]         UNIQUEIDENTIFIER    NOT NULL,
    PRIMARY KEY ([SessionId], [SpeakerId]),
    CONSTRAINT [FK_SessionSpeakers_Sessions] FOREIGN KEY ([SessionId]) REFERENCES [dbo].[Sessions] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_SessionSpeakers_Speakers] FOREIGN KEY ([SpeakerId]) REFERENCES [dbo].[Speakers] ([Id]) ON DELETE CASCADE
);

-- Index on SpeakerId
CREATE NONCLUSTERED INDEX [IX_SessionSpeakers_SpeakerId] ON [dbo].[SessionSpeakers] ([SpeakerId]);
```

### TicketTypes

Defines the types of tickets available for an event.

sql

```sql
CREATE TABLE [dbo].[TicketTypes] (
    [Id]                UNIQUEIDENTIFIER    NOT NULL PRIMARY KEY DEFAULT (NEWID()),
    [EventId]           UNIQUEIDENTIFIER    NOT NULL,
    [Name]              NVARCHAR(255)       NOT NULL,
    [Description]       NVARCHAR(MAX)       NULL,
    [Price]             DECIMAL(18, 2)      NOT NULL,
    [AvailableQuantity] INT                 NULL,  -- NULL means unlimited
    [SalesStartDate]    DATETIME2           NULL,
    [SalesEndDate]      DATETIME2           NULL,
    [Scope]             INT                 NOT NULL DEFAULT(1),  -- 1=SingleEvent, 2=AllAccess, 3=DayPass
    [CreatedAt]         DATETIME2           NOT NULL DEFAULT(GETUTCDATE()),
    [UpdatedAt]         DATETIME2           NOT NULL DEFAULT(GETUTCDATE()),
    CONSTRAINT [FK_TicketTypes_Events] FOREIGN KEY ([EventId]) REFERENCES [dbo].[Events] ([Id]) ON DELETE CASCADE
);

-- Index on EventId
CREATE NONCLUSTERED INDEX [IX_TicketTypes_EventId] ON [dbo].[TicketTypes] ([EventId]);
```

### Registrations

Tracks user registrations for events.

sql

```sql
CREATE TABLE [dbo].[Registrations] (
    [Id]                UNIQUEIDENTIFIER    NOT NULL PRIMARY KEY DEFAULT (NEWID()),
    [EventId]           UNIQUEIDENTIFIER    NOT NULL,
    [UserId]            UNIQUEIDENTIFIER    NOT NULL,
    [RegistrationDate]  DATETIME2           NOT NULL DEFAULT(GETUTCDATE()),
    [Status]            INT                 NOT NULL DEFAULT(1),  -- 1=Pending, 2=Confirmed, 3=CheckedIn, 4=Cancelled
    [Notes]             NVARCHAR(MAX)       NULL,
    [CreatedAt]         DATETIME2           NOT NULL DEFAULT(GETUTCDATE()),
    [UpdatedAt]         DATETIME2           NOT NULL DEFAULT(GETUTCDATE()),
    CONSTRAINT [FK_Registrations_Events] FOREIGN KEY ([EventId]) REFERENCES [dbo].[Events] ([Id]),
    CONSTRAINT [FK_Registrations_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
);

-- Indexes
CREATE NONCLUSTERED INDEX [IX_Registrations_EventId] ON [dbo].[Registrations] ([EventId]);
CREATE NONCLUSTERED INDEX [IX_Registrations_UserId] ON [dbo].[Registrations] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_Registrations_Status] ON [dbo].[Registrations] ([Status]);
CREATE UNIQUE INDEX [IX_Registrations_EventId_UserId] ON [dbo].[Registrations] ([EventId], [UserId]);
```

### RegistrationSessions

Tracks which sessions a user is registered for.

sql

```sql
CREATE TABLE [dbo].[RegistrationSessions] (
    [RegistrationId]    UNIQUEIDENTIFIER    NOT NULL,
    [SessionId]         UNIQUEIDENTIFIER    NOT NULL,
    PRIMARY KEY ([RegistrationId], [SessionId]),
    CONSTRAINT [FK_RegistrationSessions_Registrations] FOREIGN KEY ([RegistrationId]) REFERENCES [dbo].[Registrations] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RegistrationSessions_Sessions] FOREIGN KEY ([SessionId]) REFERENCES [dbo].[Sessions] ([Id]) ON DELETE CASCADE
);

-- Index on SessionId
CREATE NONCLUSTERED INDEX [IX_RegistrationSessions_SessionId] ON [dbo].[RegistrationSessions] ([SessionId]);
```

### Tickets

Stores issued tickets for registrations.

sql

```sql
CREATE TABLE [dbo].[Tickets] (
    [Id]                UNIQUEIDENTIFIER    NOT NULL PRIMARY KEY DEFAULT (NEWID()),
    [RegistrationId]    UNIQUEIDENTIFIER    NOT NULL,
    [TicketTypeId]      UNIQUEIDENTIFIER    NOT NULL,
    [TicketNumber]      NVARCHAR(50)        NOT NULL UNIQUE,
    [PurchasePrice]     DECIMAL(18, 2)      NOT NULL,
    [PurchaseDate]      DATETIME2           NOT NULL DEFAULT(GETUTCDATE()),
    [IsUsed]            BIT                 NOT NULL DEFAULT(0),
    [UsedDate]          DATETIME2           NULL,
    [CreatedAt]         DATETIME2           NOT NULL DEFAULT(GETUTCDATE()),
    [UpdatedAt]         DATETIME2           NOT NULL DEFAULT(GETUTCDATE()),
    CONSTRAINT [FK_Tickets_Registrations] FOREIGN KEY ([RegistrationId]) REFERENCES [dbo].[Registrations] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Tickets_TicketTypes] FOREIGN KEY ([TicketTypeId]) REFERENCES [dbo].[TicketTypes] ([Id])
);

-- Indexes
CREATE NONCLUSTERED INDEX [IX_Tickets_RegistrationId] ON [dbo].[Tickets] ([RegistrationId]);
CREATE NONCLUSTERED INDEX [IX_Tickets_TicketTypeId] ON [dbo].[Tickets] ([TicketTypeId]);
CREATE NONCLUSTERED INDEX [IX_Tickets_TicketNumber] ON [dbo].[Tickets] ([TicketNumber]);
```

### SessionMaterials

Stores materials linked to sessions.

sql

```sql
CREATE TABLE [dbo].[SessionMaterials] (
    [Id]                UNIQUEIDENTIFIER    NOT NULL PRIMARY KEY DEFAULT (NEWID()),
    [SessionId]         UNIQUEIDENTIFIER    NOT NULL,
    [Name]              NVARCHAR(255)       NOT NULL,
    [FileUrl]           NVARCHAR(255)       NOT NULL,
    [FileType]          NVARCHAR(100)       NOT NULL,
    [UploadedById]      UNIQUEIDENTIFIER    NOT NULL,
    [CreatedAt]         DATETIME2           NOT NULL DEFAULT(GETUTCDATE()),
    [UpdatedAt]         DATETIME2           NOT NULL DEFAULT(GETUTCDATE()),
    CONSTRAINT [FK_SessionMaterials_Sessions] FOREIGN KEY ([SessionId]) REFERENCES [dbo].[Sessions] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_SessionMaterials_Users] FOREIGN KEY ([UploadedById]) REFERENCES [dbo].[Users] ([Id])
);

-- Index on SessionId
CREATE NONCLUSTERED INDEX [IX_SessionMaterials_SessionId] ON [dbo].[SessionMaterials] ([SessionId]);
```

### PaymentTransactions

Tracks payment information for registrations.

sql

```sql
CREATE TABLE [dbo].[PaymentTransactions] (
    [Id]                UNIQUEIDENTIFIER    NOT NULL PRIMARY KEY DEFAULT (NEWID()),
    [RegistrationId]    UNIQUEIDENTIFIER    NOT NULL,
    [Amount]            DECIMAL(18, 2)      NOT NULL,
    [Currency]          NVARCHAR(3)         NOT NULL DEFAULT('USD'),
    [PaymentMethod]     NVARCHAR(50)        NOT NULL,
    [TransactionId]     NVARCHAR(255)       NOT NULL,
    [Status]            INT                 NOT NULL,  -- 1=Pending, 2=Completed, 3=Failed, 4=Refunded
    [TransactionDate]   DATETIME2           NOT NULL DEFAULT(GETUTCDATE()),
    [CreatedAt]         DATETIME2           NOT NULL DEFAULT(GETUTCDATE()),
    [UpdatedAt]         DATETIME2           NOT NULL DEFAULT(GETUTCDATE()),
    CONSTRAINT [FK_PaymentTransactions_Registrations] FOREIGN KEY ([RegistrationId]) REFERENCES [dbo].[Registrations] ([Id])
);

-- Indexes
CREATE NONCLUSTERED INDEX [IX_PaymentTransactions_RegistrationId] ON [dbo].[PaymentTransactions] ([RegistrationId]);
CREATE NONCLUSTERED INDEX [IX_PaymentTransactions_TransactionId] ON [dbo].[PaymentTransactions] ([TransactionId]);
CREATE NONCLUSTERED INDEX [IX_PaymentTransactions_Status] ON [dbo].[PaymentTransactions] ([Status]);
```

### Notifications

Stores notification records sent to users.

sql

```sql
CREATE TABLE [dbo].[Notifications] (
    [Id]                UNIQUEIDENTIFIER    NOT NULL PRIMARY KEY DEFAULT (NEWID()),
    [UserId]            UNIQUEIDENTIFIER    NOT NULL,
    [Type]              INT                 NOT NULL,  -- 1=Email, 2=SMS, 3=InApp
    [Title]             NVARCHAR(255)       NOT NULL,
    [Content]           NVARCHAR(MAX)       NOT NULL,
    [Status]            INT                 NOT NULL,  -- 1=Queued, 2=Sent, 3=Failed, 4=Delivered, 5=Read
    [SentAt]            DATETIME2           NULL,
    [ReadAt]            DATETIME2           NULL,
    [CreatedAt]         DATETIME2           NOT NULL DEFAULT(GETUTCDATE()),
    [UpdatedAt]         DATETIME2           NOT NULL DEFAULT(GETUTCDATE()),
    CONSTRAINT [FK_Notifications_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
);

-- Indexes
CREATE NONCLUSTERED INDEX [IX_Notifications_UserId] ON [dbo].[Notifications] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_Notifications_Status] ON [dbo].[Notifications] ([Status]);
```

### AuditLogs

Tracks important operations for auditing purposes.

sql

```sql
CREATE TABLE [dbo].[AuditLogs] (
    [Id]                UNIQUEIDENTIFIER    NOT NULL PRIMARY KEY DEFAULT (NEWID()),
    [UserId]            UNIQUEIDENTIFIER    NULL,
    [EntityType]        NVARCHAR(50)        NOT NULL,
    [EntityId]          UNIQUEIDENTIFIER    NOT NULL,
    [Action]            NVARCHAR(50)        NOT NULL,
    [OldValues]         NVARCHAR(MAX)       NULL,
    [NewValues]         NVARCHAR(MAX)       NULL,
    [IPAddress]         NVARCHAR(50)        NULL,
    [UserAgent]         NVARCHAR(255)       NULL,
    [CreatedAt]         DATETIME2           NOT NULL DEFAULT(GETUTCDATE())
);

-- Indexes
CREATE NONCLUSTERED INDEX [IX_AuditLogs_UserId] ON [dbo].[AuditLogs] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_AuditLogs_EntityType_EntityId] ON [dbo].[AuditLogs] ([EntityType], [EntityId]);
CREATE NONCLUSTERED INDEX [IX_AuditLogs_CreatedAt] ON [dbo].[AuditLogs] ([CreatedAt]);
```

## Stored Procedures

### `usp_GetEventRegistrationCount`

Returns the current registration count for an event.

sql

```sql
CREATE PROCEDURE [dbo].[usp_GetEventRegistrationCount]
    @EventId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) AS RegistrationCount
    FROM [dbo].[Registrations]
    WHERE [EventId] = @EventId
    AND [Status] IN (2, 3);  -- Confirmed or CheckedIn
END
```

### `usp_CheckEventCapacity`

Checks if an event has reached its maximum capacity.

sql

```sql
CREATE PROCEDURE [dbo].[usp_CheckEventCapacity]
    @EventId UNIQUEIDENTIFIER,
    @HasCapacity BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @MaxAttendees INT;
    DECLARE @CurrentCount INT;
    
    SELECT @MaxAttendees = [MaxAttendees]
    FROM [dbo].[Events]
    WHERE [Id] = @EventId;
    
    SELECT @CurrentCount = COUNT(*)
    FROM [dbo].[Registrations]
    WHERE [EventId] = @EventId
    AND [Status] IN (2, 3);  -- Confirmed or CheckedIn
    
    IF @MaxAttendees = 0 OR @CurrentCount < @MaxAttendees
        SET @HasCapacity = 1;
    ELSE
        SET @HasCapacity = 0;
END
```

### `usp_CreateTicket`

Creates a new ticket for a registration.

sql

```sql
CREATE PROCEDURE [dbo].[usp_CreateTicket]
    @RegistrationId UNIQUEIDENTIFIER,
    @TicketTypeId UNIQUEIDENTIFIER,
    @PurchasePrice DECIMAL(18, 2)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TicketNumber NVARCHAR(50);
    DECLARE @EventId UNIQUEIDENTIFIER;
    DECLARE @EventName NVARCHAR(255);
    DECLARE @Counter INT;
    
    -- Get event info
    SELECT @EventId = [EventId]
    FROM [dbo].[Registrations]
    WHERE [Id] = @RegistrationId;
    
    SELECT @EventName = [Name]
    FROM [dbo].[Events]
    WHERE [Id] = @EventId;
    
    -- Generate ticket number
    SELECT @Counter = COUNT(*) + 1
    FROM [dbo].[Tickets] t
    JOIN [dbo].[Registrations] r ON t.[RegistrationId] = r.[Id]
    WHERE r.[EventId] = @EventId;
    
    SET @TicketNumber = LEFT(REPLACE(@EventName, ' ', ''), 4) + 
                       CAST(YEAR(GETUTCDATE()) AS NVARCHAR(4)) + '-' +
                       RIGHT('000' + CAST(@Counter AS NVARCHAR(10)), 4);
    
    -- Insert ticket
    INSERT INTO [dbo].[Tickets] (
        [Id],
        [RegistrationId],
        [TicketTypeId],
        [TicketNumber],
        [PurchasePrice],
        [PurchaseDate]
    )
    VALUES (
        NEWID(),
        @RegistrationId,
        @TicketTypeId,
        @TicketNumber,
        @PurchasePrice,
        GETUTCDATE()
    );
    
    -- Return the created ticket
    SELECT * FROM [dbo].[Tickets]
    WHERE [RegistrationId] = @RegistrationId
    AND [TicketTypeId] = @TicketTypeId
    ORDER BY [CreatedAt] DESC;
END
```

### `usp_GetSessionConflicts`

Identifies conflicting sessions for scheduling.

sql

```sql
CREATE PROCEDURE [dbo].[usp_GetSessionConflicts]
    @SessionId UNIQUEIDENTIFIER,
    @StartTime DATETIME2,
    @EndTime DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @EventId UNIQUEIDENTIFIER;
    
    SELECT @EventId = [EventId]
    FROM [dbo].[Sessions]
    WHERE [Id] = @SessionId;
    
    SELECT s.[Id], s.[Title], s.[StartTime], s.[EndTime]
    FROM [dbo].[Sessions] s
    WHERE s.[EventId] = @EventId
    AND s.[Id] <> @SessionId
    AND (
        (@StartTime >= s.[StartTime] AND @StartTime < s.[EndTime]) OR
        (@EndTime > s.[StartTime] AND @EndTime <= s.[EndTime]) OR
        (@StartTime <= s.[StartTime] AND @EndTime >= s.[EndTime])
    );
END
```

## Views

### `vw_EventSummary`

Provides a summary view of events with registration counts.

sql

```sql
CREATE VIEW [dbo].[vw_EventSummary]
AS
SELECT
    e.[Id],
    e.[Name],
    e.[Type],
    e.[StartDate],
    e.[EndDate],
    e.[Location],
    e.[Status],
    e.[IsPublished],
    o.[OrganizationName] AS OrganizerName,
    (
        SELECT COUNT(*)
        FROM [dbo].[Registrations] r
        WHERE r.[EventId] = e.[Id]
        AND r.[Status] IN (2, 3)  -- Confirmed or CheckedIn
    ) AS ConfirmedRegistrations,
    (
        SELECT COUNT(*)
        FROM [dbo].[Sessions] s
        WHERE s.[EventId] = e.[Id]
    ) AS SessionCount
FROM
    [dbo].[Events] e
JOIN
    [dbo].[Organizers] o ON e.[OrganizerId] = o.[Id];
```

### `vw_UserRegistrationHistory`

Shows a user's registration history with event details.

sql

```sql
CREATE VIEW [dbo].[vw_UserRegistrationHistory]
AS
SELECT
    u.[Id] AS UserId,
    u.[Email],
    u.[FirstName] + ' ' + u.[LastName] AS FullName,
    r.[Id] AS RegistrationId,
    r.[RegistrationDate],
    r.[Status] AS RegistrationStatus,
    e.[Id] AS EventId,
    e.[Name] AS EventName,
    e.[StartDate] AS EventStartDate,
    e.[EndDate] AS EventEndDate,
    tt.[Name] AS TicketTypeName,
    t.[TicketNumber],
    t.[PurchasePrice],
    t.[IsUsed]
FROM
    [dbo].[Users] u
JOIN
    [dbo].[Registrations] r ON u.[Id] = r.[UserId]
JOIN
    [dbo].[Events] e ON r.[EventId] = e.[Id]
LEFT JOIN
    [dbo].[Tickets] t ON r.[Id] = t.[RegistrationId]
LEFT JOIN
    [dbo].[TicketTypes] tt ON t.[TicketTypeId] = tt.[Id];
```

### `vw_EventRevenue`

Summarizes revenue data by event.

sql

```sql
CREATE VIEW [dbo].[vw_EventRevenue]
AS
SELECT
    e.[Id] AS EventId,
    e.[Name] AS EventName,
    o.[OrganizationName] AS OrganizerName,
    COUNT(DISTINCT r.[Id]) AS TotalRegistrations,
    SUM(t.[PurchasePrice]) AS TotalRevenue,
    AVG(t.[PurchasePrice]) AS AverageTicketPrice,
    MIN(t.[PurchaseDate]) AS FirstTicketSoldDate,
    MAX(t.[PurchaseDate]) AS LastTicketSoldDate
FROM
    [dbo].[Events] e
JOIN
    [dbo].[Organizers] o ON e.[OrganizerId] = o.[Id]
JOIN
    [dbo].[Registrations] r ON e.[Id] = r.[EventId]
JOIN
    [dbo].[Tickets] t ON r.[Id] = t.[RegistrationId]
WHERE
    r.[Status] IN (2, 3)  -- Confirmed or CheckedIn
GROUP BY
    e.[Id], e.[Name], o.[OrganizationName];
```

## Database Indices

### Clustered Indices

All primary key constraints use clustered indices by default.

### Non-Clustered Indices

Non-clustered indices have been created for commonly queried columns and foreign keys to optimize query performance:

- User email (for authentication)
- Event start dates (for date range queries)
- Event status and published flag (for filtering active events)
- Registration status (for filtering by status)
- Speaker and session relationships (for efficiently loading speakers for sessions)
- Ticket numbers (for quick lookup during check-in)

### Composite Indices

Composite indices are used for columns frequently queried together:

- Event status and publication status
- Event and user for registration uniqueness check

## Constraints

### Primary Key Constraints

Every table has a primary key, typically using a GUID (uniqueidentifier) data type.

### Foreign Key Constraints

Foreign key constraints are used to maintain referential integrity between related tables. Most have cascade delete behavior where appropriate:

- Session to Event (cascade delete: when an event is deleted, its sessions are deleted)
- Registration to Event (no cascade: registrations are preserved for historical records)
- Ticket to Registration (cascade delete: when a registration is deleted, its tickets are deleted)

### Unique Constraints

Unique constraints prevent duplicate entries:

- User email addresses
- Ticket numbers
- User registrations for the same event (a user can only register once per event)

### Check Constraints

To add check constraints for enhanced data validation:

sql

```sql
-- Ensure event end date is after start date
ALTER TABLE [dbo].[Events]
ADD CONSTRAINT [CK_Events_DateRange] CHECK ([EndDate] IS NULL OR [StartDate] IS NULL OR [EndDate] >= [StartDate]);

-- Ensure session end time is after start time
ALTER TABLE [dbo].[Sessions]
ADD CONSTRAINT [CK_Sessions_TimeRange] CHECK ([EndTime] > [StartTime]);

-- Ensure ticket price is non-negative
ALTER TABLE [dbo].[TicketTypes]
ADD CONSTRAINT [CK_TicketTypes_Price] CHECK ([Price] >= 0);
```

## Database Diagrams

```
-- Major Entity Relationships

Events <---> Organizers
  |
  ↓
Sessions <---> Speakers
  |
  ↓
Registrations <---> Users
  |
  ↓
Tickets <---> TicketTypes
```

```
-- Authentication Hierarchy

Users
  |
  ↓
UserRoles
  |
  ↓
Speakers / Organizers (Profile data)
```

## Performance Considerations

1. **Partitioning Strategy**: For large deployments, consider partitioning the Events and Registrations tables by date ranges.
2. **Temporal Tables**: Consider using SQL Server's temporal tables for the Events and Registrations tables to maintain a history of changes.
3. **Indexing Strategy**: The current indexing strategy focuses on common query patterns, but should be adjusted based on actual query performance metrics.
4. **Query Optimization**: Use query hints where appropriate for complex queries, especially those joining multiple tables.
5. **Data Archiving**: Implement a data archiving strategy for completed events older than a certain timeframe to maintain optimal performance.