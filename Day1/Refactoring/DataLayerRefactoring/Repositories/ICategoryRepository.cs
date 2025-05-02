using DataLayerRefactoring.Models;

namespace DataLayerRefactoring.Repositories;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<Category> CreateAsync(Category category);
    Task<Category?> UpdateAsync(int id, Category updatedCategory);
    Task<bool> DeleteAsync(int id);
}