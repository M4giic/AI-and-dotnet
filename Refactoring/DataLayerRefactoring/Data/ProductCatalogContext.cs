using DataLayerRefactoring.Models;
using Microsoft.EntityFrameworkCore;

namespace DataLayerRefactoring.Data
{
    public class ProductCatalogContext : DbContext
    {
        public ProductCatalogContext(DbContextOptions<ProductCatalogContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            // Seed some data
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics", Description = "Electronic devices and accessories" },
                new Category { Id = 2, Name = "Clothing", Description = "Apparel and fashion items" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Smartphone", Description = "Latest model smartphone", Price = 699.99m, Stock = 100, CategoryId = 1 },
                new Product { Id = 2, Name = "Laptop", Description = "High-performance laptop", Price = 1299.99m, Stock = 50, CategoryId = 1 },
                new Product { Id = 3, Name = "T-Shirt", Description = "Cotton t-shirt", Price = 19.99m, Stock = 200, CategoryId = 2 }
            );
        }
    }
}