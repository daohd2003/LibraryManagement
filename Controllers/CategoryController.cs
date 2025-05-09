using LibraryManagement.DTOs.Request;
using LibraryManagement.DTOs.Response;
using LibraryManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net;

namespace LibraryManagement.Controllers
{
    [Route("api/v1/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(
            ICategoryService categoryService,
            ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all categories");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An error occurred while retrieving categories");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetById(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);

                if (category == null)
                {
                    _logger.LogWarning("Category with ID {CategoryId} not found", id);
                    return NotFound();
                }

                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category with ID {CategoryId}", id);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    $"An error occurred while retrieving category with ID {id}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryDto>> AddCategory(CreateCategoryDto dto)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for adding category");
                return BadRequest(ModelState);
            }

            var addedCategory = await _categoryService.CreateCategoryAsync(dto);
            return CreatedAtAction(
                nameof(GetById),
                new { id = addedCategory.Id },
                addedCategory);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for category update");
                return BadRequest(ModelState);
            }
            var result = await _categoryService.UpdateCategoryAsync(id, dto);

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            return Ok();
        }
    }
}