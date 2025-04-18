using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventManagement.Core.Entities
{
    public class Event
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Venue is required")]
        public string VenueName { get; set; }

        [Range(1, 100000, ErrorMessage = "Capacity must be between 1 and 100,000")]
        public int Capacity { get; set; }

        public EventType Type { get; set; }

        public int? ParentEventId { get; set; }
        public Event ParentEvent { get; set; }

        public ICollection<Event> SubEvents { get; set; } = new List<Event>();

        public EventStatus Status { get; set; }

        public string? BannerImageUrl { get; set; }

        // Navigation property
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }

    public enum EventType
    {
        Standalone,
        Series,
        SubEvent
    }

    public enum EventStatus
    {
        Draft,
        Published,
        Canceled,
        Completed
    }
}