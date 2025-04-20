using DataLayerRefactoring.Models;
using Microsoft.EntityFrameworkCore;

namespace DataLayerRefactoring.Data;

public class DataSeeder
{
    private readonly ProductCatalogContext _context;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(ProductCatalogContext context, ILogger<DataSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedData()
    {
        try
        {
            // Only seed if the database is empty
            if (!await _context.Categories.AnyAsync() && !await _context.Products.AnyAsync())
            {
                _logger.LogInformation("Starting to seed the database...");

                // Add categories
                var electronics = new Category
                {
                    Name = "Electronics",
                    Description = "Electronic devices and accessories",
                };

                var clothing = new Category
                {
                    Name = "Clothing",
                    Description = "Apparel and fashion items",
                };

                var furniture = new Category
                {
                    Name = "Furniture",
                    Description = "Home and office furniture",
                };

                await _context.Categories.AddRangeAsync(electronics, clothing, furniture);
                await _context.SaveChangesAsync();

                // Add products
                var products = new[]
                {
                    new Product
                    {
                        Name = "Smartphone",
                        Description = "Latest model smartphone with advanced features",
                        Price = 699.99m,
                        Stock = 100,
                        CategoryId = electronics.Id,
                    },
                    new Product
                    {
                        Name = "Laptop",
                        Description = "High-performance laptop for professionals",
                        Price = 1299.99m,
                        Stock = 50,
                        CategoryId = electronics.Id,
                    },
                    new Product
                    {
                        Name = "Wireless Earbuds",
                        Description = "Premium wireless earbuds with noise cancellation",
                        Price = 149.99m,
                        Stock = 75,
                        CategoryId = electronics.Id,
                    },
                    new Product
                    {
                        Name = "T-Shirt",
                        Description = "100% cotton comfortable t-shirt",
                        Price = 19.99m,
                        Stock = 200,
                        CategoryId = clothing.Id,
                    },
                    new Product
                    {
                        Name = "Jeans",
                        Description = "Classic denim jeans",
                        Price = 49.99m,
                        Stock = 150,
                        CategoryId = clothing.Id,
                    },
                    new Product
                    {
                        Name = "Office Chair",
                        Description = "Ergonomic office chair with lumbar support",
                        Price = 199.99m,
                        Stock = 30,
                        CategoryId = furniture.Id,
                    },
                    new Product
                    {
                        Name = "Desk",
                        Description = "Modern computer desk with storage",
                        Price = 249.99m,
                        Stock = 25,
                        CategoryId = furniture.Id,
                    }
                };

                await _context.Products.AddRangeAsync(products);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Database seeded successfully");
            }
            else
            {
                _logger.LogInformation("Database already contains data - skipping seed");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }
}