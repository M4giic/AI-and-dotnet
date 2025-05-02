using EventManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.API.Data;

public class EventManagementDbContext : DbContext
{
    public EventManagementDbContext(DbContextOptions<EventManagementDbContext> options)
        : base(options)
    {
    }

    public DbSet<Event> Events { get; set; }
    public DbSet<Session> Sessions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Event entity
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.VenueName).IsRequired();
                
            // Configure relationship with parent event
            entity.HasOne(e => e.ParentEvent)
                .WithMany(e => e.SubEvents)
                .HasForeignKey(e => e.ParentEventId)
                .OnDelete(DeleteBehavior.Restrict); // Don't cascade delete to avoid circular dependencies
        });

        // Configure Session entity
        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Title).IsRequired().HasMaxLength(100);
                
            // Configure relationship with event
            entity.HasOne(s => s.Event)
                .WithMany(e => e.Sessions)
                .HasForeignKey(s => s.EventId)
                .OnDelete(DeleteBehavior.Cascade); // Delete sessions when event is deleted
        });

        // Seed some initial data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Add some sample events
        modelBuilder.Entity<Event>().HasData(
            new Event
            {
                Id = 1,
                Title = "Tech Conference 2025",
                Description = "Annual technology conference featuring the latest innovations and industry trends.",
                StartDate = new System.DateTime(2025, 6, 1),
                EndDate = new System.DateTime(2025, 6, 3),
                VenueName = "Tech Convention Center",
                Capacity = 1000,
                Type = EventType.Series,
                Status = EventStatus.Published,
                BannerImageUrl = "/sample-images/tech-conference.jpg"
            },
            new Event
            {
                Id = 2,
                Title = "Keynote: Future of AI",
                Description = "Opening keynote discussing the future of artificial intelligence and its impact on society.",
                StartDate = new System.DateTime(2025, 6, 1, 9, 0, 0),
                EndDate = new System.DateTime(2025, 6, 1, 10, 30, 0),
                VenueName = "Main Hall",
                Capacity = 1000,
                Type = EventType.SubEvent,
                ParentEventId = 1,
                Status = EventStatus.Published
            },
            new Event
            {
                Id = 3,
                Title = "Workshop: Building with Blazor",
                Description = "Hands-on workshop on developing applications with Blazor and .NET.",
                StartDate = new System.DateTime(2025, 6, 2, 13, 0, 0),
                EndDate = new System.DateTime(2025, 6, 2, 16, 0, 0),
                VenueName = "Workshop Room A",
                Capacity = 50,
                Type = EventType.SubEvent,
                ParentEventId = 1,
                Status = EventStatus.Published,            

            },
            new Event
            {
                Id = 4,
                Title = "Summer Concert",
                Description = "Annual summer concert featuring local bands and artists.",
                StartDate = new System.DateTime(2025, 7, 15, 18, 0, 0),
                EndDate = new System.DateTime(2025, 7, 15, 22, 0, 0),
                VenueName = "City Park Amphitheater",
                Capacity = 500,
                Type = EventType.Standalone,
                Status = EventStatus.Draft,
                BannerImageUrl = "/sample-images/summer-concert.jpg"
            }
        );
    }
}