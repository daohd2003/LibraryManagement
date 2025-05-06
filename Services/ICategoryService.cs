using LibraryManagement.DTOs.Request;
using LibraryManagement.DTOs.Response;
using LibraryManagement.Models;

namespace LibraryManagement.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(int id);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto category);
        Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDto category);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
