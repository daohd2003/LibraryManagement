using AutoMapper;
using LibraryManagement.DTOs.Request;
using LibraryManagement.DTOs.Response;
using LibraryManagement.Exceptions;
using LibraryManagement.Models;
using LibraryManagement.Repositories;
using System.Data;

namespace LibraryManagement.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);

            if (await _categoryRepository.ExistsByNameAsync(categoryDto.Name))
            {
                throw new DuplicateCategoryException(categoryDto.Name);
            }
            var addedCategory = await _categoryRepository.AddAsync(category);

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.ExistsAsync(id);

            if (!category)
            {
                throw new NotFoundCategoryException();
            }
            return await _categoryRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var category = await _categoryRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<CategoryDto>>(category);
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return _mapper?.Map<CategoryDto?>(category);
        }

        public async Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
        {
            if (id != dto.Id)
            {
                throw new MismatchedCategoryIdException(id, dto.Id);
            }

            var existingCategory = await _categoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
            {
                throw new NotFoundCategoryException();
            }

            var categories = await _categoryRepository.ExistsByNameAsync(dto.Name);

            if (categories)
            {
                throw new DuplicateCategoryException(dto.Name);
            }

            Category category;
            try
            {
                category = _mapper.Map<Category>(dto);
            }
            catch (Exception)
            {
                throw new MappingException("Mapping failed");
            }

            return await _categoryRepository.UpdateAsync(category);
        }
    }
}
