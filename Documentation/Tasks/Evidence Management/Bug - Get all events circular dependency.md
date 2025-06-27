## Issue Description

When calling the `/api/Events` endpoint, the application is throwing a JSON serialization exception due to a circular reference in the object graph. This occurs because parent events reference sub-events, which in turn reference their parent event, creating an infinite loop during serialization.

## Error Message

```
System.Text.Json.JsonException: A possible object cycle was detected. This can either be due to a cycle or if the object depth is larger than the maximum allowed depth of 32. Consider using ReferenceHandler.Preserve on JsonSerializerOptions to support cycles. Path: $.SubEvents.ParentEvent.SubEvents.ParentEvent.SubEvents.ParentEvent.SubEvents.ParentEvent.SubEvents.ParentEvent.SubEvents.ParentEvent.SubEvents.ParentEvent.SubEvents.ParentEvent.SubEvents.ParentEvent.Id.
```

## Technical Details

- The JSON serializer is detecting a circular reference between parent events and sub-events
- Each Event has a navigation property to its parent (ParentEvent) and a collection of children (SubEvents)
- When serializing, this creates an infinite nesting pattern:
    - Event → SubEvents → ParentEvent (back to original Event) → SubEvents →...

## Expected Behavior

The API should return a properly formatted JSON response without serialization errors, appropriately handling the parent-child relationship between events.

## Steps to Reproduce

1. Start the API application
2. Make a GET request to the `/api/Events` endpoint
3. Observe the exception being thrown

## Acceptance Criteria

- The `/api/Events` endpoint returns a valid JSON response without errors
- The event hierarchy is maintained in a way that's useful for the client
- No circular references in the serialized output
- Response contains all necessary event data for the client to use