# Event Management Platform - Domain Models

## Core Domain Models

### Event

The `Event` entity represents the primary domain concept within the system. Events can be either standalone events or series events containing multiple sub-events.

```csharp
public class Event
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public EventType Type { get; private set; }  // Standalone or Series
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public string Location { get; private set; }
    public string VirtualMeetingUrl { get; private set; }
    public int MaxAttendees { get; private set; }
    public bool IsPublished { get; private set; }
    public Guid OrganizerId { get; private set; }
    public EventStatus Status { get; private set; }
    
    // For series events
    public Guid? ParentEventId { get; private set; }
    public Event ParentEvent { get; private set; }
    public ICollection<Event> SubEvents { get; private set; }
    
    // Relationships
    public Organizer Organizer { get; private set; }
    public ICollection<Session> Sessions { get; private set; }
    public ICollection<TicketType> TicketTypes { get; private set; }
    public ICollection<Registration> Registrations { get; private set; }
    
    // Domain behavior methods
    public void Publish()
    {
        // Validation logic before publishing
        if (string.IsNullOrEmpty(Name) || StartDate == null)
            throw new DomainException("Cannot publish an event without a name and start date");
            
        IsPublished = true;
        Status = EventStatus.Published;
        
        // Raise domain event
        DomainEvents.Raise(new EventPublishedEvent(this));
    }
    
    public void Cancel(string reason)
    {
        Status = EventStatus.Cancelled;
        
        // Raise domain event with cancellation reason
        DomainEvents.Raise(new EventCancelledEvent(this, reason));
    }
    
    public bool CanRegister(DateTime registrationDate)
    {
        return IsPublished && 
               Status == EventStatus.Published &&
               (MaxAttendees == 0 || Registrations.Count < MaxAttendees) &&
               (StartDate == null || registrationDate < StartDate);
    }
    
    public void AddSubEvent(Event subEvent)
    {
        if (Type != EventType.Series)
            throw new DomainException("Cannot add sub-events to a standalone event");
            
        subEvent.ParentEventId = this.Id;
        SubEvents.Add(subEvent);
    }
}

public enum EventType
{
    Standalone,
    Series
}

public enum EventStatus
{
    Draft,
    Published,
    Cancelled,
    Completed
}
```

### Session

The `Session` entity represents a scheduled activity within an event.

```csharp
public class Session
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public int MaxAttendees { get; private set; }
    public string Location { get; private set; }
    public string VirtualMeetingUrl { get; private set; }
    
    // Relationships
    public Guid EventId { get; private set; }
    public Event Event { get; private set; }
    public ICollection<Speaker> Speakers { get; private set; }
    public ICollection<SessionAttendee> Attendees { get; private set; }
    
    // Domain behavior
    public bool HasTimeConflict(Session otherSession)
    {
        if (otherSession.EventId != this.EventId) 
            return false;
            
        return (this.StartTime <= otherSession.EndTime && 
                this.EndTime >= otherSession.StartTime);
    }
    
    public bool CanRegister(Registration registration)
    {
        return MaxAttendees == 0 || Attendees.Count < MaxAttendees;
    }
}
```

### TicketType

The `TicketType` entity defines the various ticket options available for an event.

```csharp
public class TicketType
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public int? AvailableQuantity { get; private set; }  // null means unlimited
    public DateTime? SalesStartDate { get; private set; }
    public DateTime? SalesEndDate { get; private set; }
    public TicketScope Scope { get; private set; }
    
    // Relationships
    public Guid EventId { get; private set; }
    public Event Event { get; private set; }
    public ICollection<Ticket> IssuedTickets { get; private set; }
    
    // Domain behavior
    public bool IsAvailable(DateTime purchaseDate)
    {
        if (AvailableQuantity.HasValue && IssuedTickets.Count >= AvailableQuantity.Value)
            return false;
            
        if (SalesStartDate.HasValue && purchaseDate < SalesStartDate.Value)
            return false;
            
        if (SalesEndDate.HasValue && purchaseDate > SalesEndDate.Value)
            return false;
            
        return true;
    }
}

public enum TicketScope
{
    SingleEvent,  // Valid for a specific event only
    AllAccess,    // Valid for all sub-events in a series
    DayPass       // Valid for all events on a specific day
}
```

### Registration

The `Registration` entity tracks a user's registration for an event.

```csharp
public class Registration
{
    public Guid Id { get; private set; }
    public DateTime RegistrationDate { get; private set; }
    public RegistrationStatus Status { get; private set; }
    public string Notes { get; private set; }
    
    // Relationships
    public Guid EventId { get; private set; }
    public Event Event { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; }
    public ICollection<Ticket> Tickets { get; private set; }
    
    // Domain behavior
    public void Confirm()
    {
        if (Status != RegistrationStatus.Pending)
            throw new DomainException("Only pending registrations can be confirmed");
            
        Status = RegistrationStatus.Confirmed;
        
        // Raise domain event
        DomainEvents.Raise(new RegistrationConfirmedEvent(this));
    }
    
    public void Cancel(string reason)
    {
        Status = RegistrationStatus.Cancelled;
        
        // Raise domain event
        DomainEvents.Raise(new RegistrationCancelledEvent(this, reason));
    }
    
    public bool CheckIn()
    {
        if (Status != RegistrationStatus.Confirmed)
            return false;
            
        Status = RegistrationStatus.CheckedIn;
        
        // Raise domain event
        DomainEvents.Raise(new AttendeeCheckedInEvent(this));
        return true;
    }
}

public enum RegistrationStatus
{
    Pending,
    Confirmed,
    CheckedIn,
    Cancelled
}
```

### Ticket

The `Ticket` entity represents a purchased ticket for an event.

```csharp
public class Ticket
{
    public Guid Id { get; private set; }
    public string TicketNumber { get; private set; }
    public decimal PurchasePrice { get; private set; }
    public DateTime PurchaseDate { get; private set; }
    public bool IsUsed { get; private set; }
    public DateTime? UsedDate { get; private set; }
    
    // Relationships
    public Guid RegistrationId { get; private set; }
    public Registration Registration { get; private set; }
    public Guid TicketTypeId { get; private set; }
    public TicketType TicketType { get; private set; }
    
    // Domain behavior
    public bool UseTicket(DateTime useDate)
    {
        if (IsUsed)
            return false;
            
        IsUsed = true;
        UsedDate = useDate;
        
        // Raise domain event
        DomainEvents.Raise(new TicketUsedEvent(this));
        return true;
    }
}
```

### User

The `User` entity represents system users with various roles.

```csharp
public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string PhoneNumber { get; private set; }
    public bool EmailConfirmed { get; private set; }
    public bool PhoneConfirmed { get; private set; }
    
    // Relationships
    public ICollection<UserRole> Roles { get; private set; }
    public ICollection<Registration> Registrations { get; private set; }
    
    // For organizers
    public Organizer OrganizerProfile { get; private set; }
    
    // For speakers
    public Speaker SpeakerProfile { get; private set; }
    
    // Domain behavior
    public string FullName => $"{FirstName} {LastName}";
    
    public bool HasRole(RoleType role)
    {
        return Roles.Any(r => r.Role == role);
    }
    
    public void AssignRole(RoleType role)
    {
        if (!HasRole(role))
        {
            Roles.Add(new UserRole { UserId = Id, Role = role });
        }
    }
    
    public void RemoveRole(RoleType role)
    {
        var userRole = Roles.FirstOrDefault(r => r.Role == role);
        if (userRole != null)
        {
            Roles.Remove(userRole);
        }
    }
}

public enum RoleType
{
    Administrator,
    Organizer,
    Speaker,
    Attendee
}

public class UserRole
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    public RoleType Role { get; set; }
}
```

### Organizer

The `Organizer` entity contains organizer-specific information.

```csharp
public class Organizer
{
    public Guid Id { get; private set; }
    public string OrganizationName { get; private set; }
    public string Description { get; private set; }
    public string LogoUrl { get; private set; }
    public string Website { get; private set; }
    
    // Relationships
    public Guid UserId { get; private set; }
    public User User { get; private set; }
    public ICollection<Event> OrganizedEvents { get; private set; }
}
```

### Speaker

The `Speaker` entity contains speaker-specific information.

```csharp
public class Speaker
{
    public Guid Id { get; private set; }
    public string Bio { get; private set; }
    public string PhotoUrl { get; private set; }
    public string Topics { get; private set; }
    
    // Relationships
    public Guid UserId { get; private set; }
    public User User { get; private set; }
    public ICollection<Session> Sessions { get; private set; }
}
```

## Value Objects

### Address

```csharp
public class Address : ValueObject
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string ZipCode { get; private set; }
    public string Country { get; private set; }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return ZipCode;
        yield return Country;
    }
    
    public override string ToString()
    {
        return $"{Street}, {City}, {State} {ZipCode}, {Country}";
    }
}
```

### Money

```csharp
public class Money : ValueObject
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }
    
    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
    
    public Money Add(Money money)
    {
        if (Currency != money.Currency)
            throw new DomainException("Cannot add money with different currencies");
            
        return new Money(Amount + money.Amount, Currency);
    }
    
    public Money Subtract(Money money)
    {
        if (Currency != money.Currency)
            throw new DomainException("Cannot subtract money with different currencies");
            
        return new Money(Amount - money.Amount, Currency);
    }
}
```

## Domain Events

```csharp
public class EventPublishedEvent : DomainEvent
{
    public Event Event { get; }
    
    public EventPublishedEvent(Event @event)
    {
        Event = @event;
    }
}

public class EventCancelledEvent : DomainEvent
{
    public Event Event { get; }
    public string Reason { get; }
    
    public EventCancelledEvent(Event @event, string reason)
    {
        Event = @event;
        Reason = reason;
    }
}

public class RegistrationConfirmedEvent : DomainEvent
{
    public Registration Registration { get; }
    
    public RegistrationConfirmedEvent(Registration registration)
    {
        Registration = registration;
    }
}

public class RegistrationCancelledEvent : DomainEvent
{
    public Registration Registration { get; }
    public string Reason { get; }
    
    public RegistrationCancelledEvent(Registration registration, string reason)
    {
        Registration = registration;
        Reason = reason;
    }
}

public class AttendeeCheckedInEvent : DomainEvent
{
    public Registration Registration { get; }
    
    public AttendeeCheckedInEvent(Registration registration)
    {
        Registration = registration;
    }
}

public class TicketUsedEvent : DomainEvent
{
    public Ticket Ticket { get; }
    
    public TicketUsedEvent(Ticket ticket)
    {
        Ticket = ticket;
    }
}
```

## Aggregate Roots

The main aggregate roots in the system are:

1. **Event** - The central aggregate that manages all event-related entities
2. **User** - Manages user profile and roles
3. **Registration** - Manages the registration process and tickets

These aggregates enforce consistency boundaries and encapsulate business rules related to their entities.