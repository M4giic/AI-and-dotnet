using DataLayerRefactoring.Models;
using DataLayerRefactoring.Models.DTO;
using DataLayerRefactoring.Repositories;

namespace DataLayerRefactoring.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.Select(MapToDto);
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        return category != null ? MapToDto(category) : null;
    }

    public async Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto)
    {
        var category = new Category
        {
            Name = categoryDto.Name,
            Description = categoryDto.Description
        };

        var createdCategory = await _categoryRepository.CreateAsync(category);
        return MapToDto(createdCategory);
    }

    public async Task<CategoryDto?> UpdateCategoryAsync(int id, CategoryDto categoryDto)
    {
        var existingCategory = await _categoryRepository.GetByIdAsync(id);
        if (existingCategory == null)
            return null;

        existingCategory.Name = categoryDto.Name;
        existingCategory.Description = categoryDto.Description;

        var updatedCategory = await _categoryRepository.UpdateAsync(id, existingCategory);
        return updatedCategory != null ? MapToDto(updatedCategory) : null;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        return await _categoryRepository.DeleteAsync(id);
    }

    private static CategoryDto MapToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ProductCount = category.Products.Count
        };
    }
}