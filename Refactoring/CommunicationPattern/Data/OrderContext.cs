using CommunicationPattern.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunicationPattern.Data;

public class OrderContext : DbContext
{
    public OrderContext(DbContextOptions<OrderContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the Order entity
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ProductId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.TotalPrice).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.OrderDate).IsRequired();
            entity.Property(e => e.Status).IsRequired();
        });
    }
}