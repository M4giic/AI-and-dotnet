## Issue Description

When attempting to create a standalone event through the API, a validation error occurs indicating that the "ParentEvent" field is required, even though standalone events shouldn't require a parent event.

## Error Message

json

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "ParentEvent": [
      "The ParentEvent field is required."
    ]
  },
  "traceId": "00-b26ca9ee7c65ff5eea0743f3230af758-f0e0c77228e9a63c-00"
}
```

## Technical Details

- The API is incorrectly validating the Event model
- For events of type "Standalone" (presumably EventType = 0), the ParentEvent field should be optional
- The model validation is treating ParentEvent as a required field for all event types
- This contradicts the business logic where standalone events don't have parent events

## Expected Behavior

When creating a standalone event, the ParentEvent field should be optional, and the API should accept null values for this field.

## Steps to Reproduce

1. Send a POST request to the events endpoint to create a new event
2. Set the event type to Standalone (EventType = 0)
3. Do not include a ParentEvent value or set it to null
4. Observe the validation error response
## Acceptance Criteria

- Standalone events can be created successfully without a ParentEvent
- The API accepts null or missing ParentEvent fields for EventType = 0
- Proper validation still occurs for SubEvents (EventType = 2) to ensure they have a parent
- The fix doesn't break existing functionality