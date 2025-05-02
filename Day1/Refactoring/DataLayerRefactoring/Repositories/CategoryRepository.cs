using DataLayerRefactoring.Data;
using DataLayerRefactoring.Models;
using Microsoft.EntityFrameworkCore;

namespace DataLayerRefactoring.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ProductCatalogContext _context;

    public CategoryRepository(ProductCatalogContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _context.Categories
            .Include(c => c.Products)
            .ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Category> CreateAsync(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<Category?> UpdateAsync(int id, Category updatedCategory)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return null;

        category.Name = updatedCategory.Name;
        category.Description = updatedCategory.Description;

        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return false;

        // Check if there are any products in this category
        var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == id);
        if (hasProducts)
            return false; // Cannot delete categories with products

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }
}