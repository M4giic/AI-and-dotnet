using DataLayerRefactoring.Models;
using DataLayerRefactoring.Models.DTO;
using DataLayerRefactoring.Repositories;

namespace DataLayerRefactoring.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(MapToDto);
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product != null ? MapToDto(product) : null;
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
    {
        var products = await _productRepository.GetByCategoryIdAsync(categoryId);
        return products.Select(MapToDto);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto productDto)
    {
        var product = new Product
        {
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price,
            Stock = productDto.Stock,
            CategoryId = productDto.CategoryId
        };

        var createdProduct = await _productRepository.CreateAsync(product);
        return MapToDto(createdProduct);
    }

    public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto productDto)
    {
        var existingProduct = await _productRepository.GetByIdAsync(id);
        if (existingProduct == null)
            return null;

        // Update only provided fields
        if (productDto.Name != null)
            existingProduct.Name = productDto.Name;
        if (productDto.Description != null)
            existingProduct.Description = productDto.Description;
        if (productDto.Price.HasValue)
            existingProduct.Price = productDto.Price.Value;
        if (productDto.Stock.HasValue)
            existingProduct.Stock = productDto.Stock.Value;
        if (productDto.CategoryId.HasValue)
            existingProduct.CategoryId = productDto.CategoryId.Value;

        var updatedProduct = await _productRepository.UpdateAsync(id, existingProduct);
        return updatedProduct != null ? MapToDto(updatedProduct) : null;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        return await _productRepository.DeleteAsync(id);
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? string.Empty
        };
    }
}