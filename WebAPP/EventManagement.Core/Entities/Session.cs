using System;
using System.ComponentModel.DataAnnotations;

namespace EventManagement.Core.Entities
{
    public class Session
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Start time is required")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "End time is required")]
        public DateTime EndTime { get; set; }

        public string Location { get; set; }

        [Required(ErrorMessage = "Event is required")]
        public int EventId { get; set; }
        public Event Event { get; set; }

        public string SpeakerName { get; set; }
        public string SpeakerBio { get; set; }
    }
}