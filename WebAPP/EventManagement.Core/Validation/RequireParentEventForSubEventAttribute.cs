using EventManagement.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace EventManagement.Core.Validation;

public class RequireParentEventForSubEventAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var eventEntity = (Event)validationContext.ObjectInstance;
        
        if (eventEntity.Type == EventType.SubEvent && eventEntity.ParentEventId == null)
        {
            return new ValidationResult("ParentEvent is required for sub-events");
        }

        return ValidationResult.Success;
    }
} 