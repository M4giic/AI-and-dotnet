using Xunit;
using System.Collections.Generic;

namespace BadCode.Tests;

public class ProductRepositoryTests
{
    [Fact]
    public void GetAllProducts_ReturnsInitialProducts()
    {
        var repo = new ProductRepository();
        var products = repo.GetAllProducts();
        Assert.Equal(3, products.Count);
    }

    [Fact]
    public void AddProduct_IncreasesProductCount()
    {
        var repo = new ProductRepository();
        var newProduct = new Product { Id = 4, Name = "Tablet", Price = 299.99m, Stock = 5, IsAvailable = true };
        repo.AddProduct(newProduct);
        Assert.Equal(4, repo.GetAllProducts().Count);
    }

    [Fact]
    public void UpdateProduct_UpdatesExistingProduct()
    {
        var repo = new ProductRepository();
        var updated = new Product { Id = 1, Name = "Laptop Pro", Price = 1299.99m, Stock = 8, IsAvailable = true };
        var result = repo.UpdateProduct(updated);
        Assert.True(result);
        Assert.Equal("Laptop Pro", repo.GetAllProducts()[0].Name);
    }

    [Fact]
    public void DeleteProduct_RemovesProduct()
    {
        var repo = new ProductRepository();
        repo.DeleteProduct(1);
        Assert.Equal(2, repo.GetAllProducts().Count);
    }

    [Fact]
    public void SearchProducts_ReturnsMatchingProducts()
    {
        var repo = new ProductRepository();
        var results = repo.SearchProducts("Phone");
        Assert.Single(results);
        Assert.Equal("Phone", results[0].Name);
    }

    [Fact]
    public void GetAvailableProducts_ReturnsOnlyAvailable()
    {
        var repo = new ProductRepository();
        var available = repo.GetAvailableProducts();
        Assert.Equal(2, available.Count);
        Assert.All(available, p => Assert.True(p.IsAvailable && p.Stock > 0));
    }

    [Fact]
    public void GetTotalInventoryValue_ReturnsCorrectSum()
    {
        var repo = new ProductRepository();
        var total = repo.GetTotalInventoryValue();
        Assert.Equal(999.99m * 10 + 499.99m * 20 + 99.99m * 0, total);
    }
}
