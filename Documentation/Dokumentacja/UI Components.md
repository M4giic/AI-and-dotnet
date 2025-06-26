# Event Management Platform - Blazor UI Components 

## Key Components 

### Event Components

#### EventCard Component

The `EventCard` component displays a summary of an event in a card format, used in event listings. It shows:

- Event title and brief description
- Event date and location
- Event organizer
- Event image/thumbnail if available
- Quick registration button

This component serves as a consistent representation of events throughout the application.

#### EventList Component

The `EventList` component displays a collection of `EventCard` components with:

- Grid or list view toggle
- Filtering options (date range, event type, etc.)
- Sorting capabilities
- Pagination
- Empty state handling
- List/Grid view toggle

#### EventDetails Component

This component provides a comprehensive view of an event, including:

- Complete event information
- Sub-event listings (for series events)
- Session schedule
- Speaker information
- Available ticket types
- Registration options
- Map integration for physical locations
- Virtual meeting link for online events

#### EventForm Component

A form component for creating or editing events with:

- Form validation
- Image upload capability
- Rich text editor for description
- Date/time pickers
- Sub-event management
- Location picker (with map integration)
- Virtual meeting options

#### SeriesEventManager Component

A specialized component for managing series events with:

- Sub-event creation interface
- Timeline visualization of all sub-events
- Drag-and-drop scheduling
- Bulk operations for sub-events

### Session Components

#### SessionCard Component

A card component that displays session information including:

- Session title and description
- Time and location
- Speaker information
- Attendance capacity indicator
- Registration status

#### SessionList Component

Displays sessions with:

- Filtering options
- Time-based grouping
- Track-based grouping
- Conflict detection
- Calendar view option

#### SessionForm Component

A form for creating or editing sessions with:

- Time slot selection
- Speaker assignment
- Room/location selection
- Capacity settings
- Prerequisites specification
- Materials upload

#### SessionScheduler Component

A visual scheduler for organizing sessions with:

- Timeline view
- Room/track swim lanes
- Drag-and-drop interface
- Conflict detection
- Bulk scheduling options

### Registration Components

#### RegistrationForm Component

A multi-step registration form with:

- Attendee information collection
- Ticket type selection
- Add-on options
- Session selection (where applicable)
- Promotional code input
- Terms and conditions acceptance

#### TicketSelector Component

A component for selecting ticket types with:

- Pricing display
- Availability indicators
- Quantity selector
- Ticket comparison
- Special offer highlighting

#### ConfirmationDisplay Component

Displays registration confirmation with:

- Ticket summary
- QR/Barcode for check-in
- Calendar integration options
- Share on social media options
- Email/print options

#### CheckInComponent Component

Used by event staff for attendee check-in with:

- QR/Barcode scanner integration
- Manual lookup option
- Quick attendee information display
- Check-in status indicators
- Badge printing trigger

### User Components

#### UserProfile Component

Displays and allows editing of user profile information:

- Personal details
- Contact preferences
- Password management
- Registration history
- Favorite events

#### SpeakerProfile Component

Extended profile component for speakers with:

- Bio and photo management
- Areas of expertise
- Session history
- Presentation materials

#### OrganizerProfile Component

Profile component for event organizers with:

- Organization details
- Logo management
- Team member management
- Event portfolio

### Common Components

#### Alert Component

A flexible alert component for displaying:

- Success messages
- Error messages
- Warning notifications
- Information alerts
- Dismissible options

#### Button Component

A customizable button component with:

- Multiple styles (primary, secondary, outline, etc.)
- Size variants
- Icon support
- Loading state
- Disabled state handling

#### Card Component

A versatile card component supporting:

- Header and footer
- Media sections
- Action buttons
- Hover effects
- Different width configurations

#### Dropdown Component

A reusable dropdown component with:

- Multi-select option
- Search/filter capability
- Grouping
- Custom item templates
- Keyboard navigation

#### Modal Component

A modal dialog component with:

- Various sizes
- Customizable header and footer
- Close button options
- Backdrop click handling
- Animation options

#### Pagination Component

A pagination component with:

- Page size selection
- First/last page navigation
- Current position indicator
- Responsive design

#### SearchBox Component

An enhanced search input with:

- Auto-suggestions
- Recent searches
- Advanced search options
- Clear button
- Search history

#### Tabs Component

A tab navigation component with:

- Horizontal and vertical orientation
- Badge support
- Icon support
- Lazy loading of content
- Responsive behavior

#### Toast Component

A notification toast system with:

- Multiple toast types (success, error, warning, info)
- Auto-dismiss option
- Position configuration
- Animation effects
- Queue management

### Form Components

#### DateTimePicker Component

A comprehensive date and time picker with:

- Date range selection
- Time selection
- Multiple calendar views
- Localization support
- Minimum/maximum date constraints

#### FileUpload Component

A file upload component with:

- Drag and drop support
- Multiple file selection
- Progress indication
- File type validation
- Size limitation
- Preview capability

#### InputMoney Component

A specialized input for monetary values with:

- Currency selection
- Formatting
- Validation
- Decimal precision control

#### MultiSelect Component

An enhanced selection component with:

- Search functionality
- Check/uncheck all
- Grouping
- Custom item templates
- Selected item chips/tags

#### RichTextEditor Component

A WYSIWYG editor component with:

- Formatting options
- Image embedding
- Link creation
- Table support
- Source code view

#### ValidationMessage Component

An enhanced validation message component with:

- Contextual styling
- Icon support
- Tooltip option
- Error aggregation

## Component Customization

### Theme Support

All components support theme customization through:

- CSS variables for colors and dimensions
- Dark/light mode toggle
- Responsive breakpoints
- Accessibility considerations

### Internationalization

Components support internationalization through:

- Text resource externalization
- Right-to-left (RTL) layout support
- Date and number formatting based on culture
- Pluralization rules

### Accessibility

Components meet WCAG 2.1 AA standards with:

- Proper ARIA attributes
- Keyboard navigation
- Screen reader compatibility
- Focus management
- Color contrast compliance

## Component Communication Patterns

### Component Parameters

Component parameters follow these conventions:

- Required parameters are marked with `[Parameter(CascadingValue = true)]`
- Optional parameters provide sensible defaults
- Complex objects use well-defined DTOs
- Event callbacks use EventCallback<T>

### Cascading Parameters

Cascading parameters are used for:

- Theme settings
- Authentication state
- Localization preferences
- Application-wide configuration

### Event Callbacks

Event callbacks follow these conventions:

- Naming: `On{Event}` (e.g., OnSubmit, OnChange)
- Return types: void or Task
- Parameters: strongly typed event args
- Error handling within components

### State Management

Components interact with application state through:

- Fluxor state containers
- Service injection
- Cascading parameters
- Local component state as appropriate

## Component Documentation

Each component includes:

- XML documentation comments
- Example usage
- Parameter descriptions
- Event descriptions
- Dependency notes
- Accessibility considerations