using DataLayerRefactoring.Models;

namespace DataLayerRefactoring.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId);
    Task<Product> CreateAsync(Product product);
    Task<Product?> UpdateAsync(int id, Product updatedProduct);
    Task<bool> DeleteAsync(int id);
}