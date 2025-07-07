using Microsoft.AspNetCore.Mvc;
using BusinessObjects.Entities;
using Repositories.DTOs;
using BusinessObjects.Commons;
using Repositories.Interfaces;

namespace GroceryAPI.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryService;

        public CategoryController(ICategoryRepository categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/categories/get-all
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            var categories = _categoryService.GetAll();
            return Ok(new ApiResponse<List<Category>>
            {
                Success = true,
                Message = "Category list retrieved.",
                Data = categories
            });
        }

        // GET: api/categories/get-by-id/{id}
        [HttpGet("get-by-id/{id}")]
        public IActionResult GetById(int id)
        {
            var category = _categoryService.GetById(id);
            if (category == null)
            {
                return NotFound(new ApiResponse<Category>
                {
                    Success = false,
                    Message = "Category not found"
                });
            }

            return Ok(new ApiResponse<Category>
            {
                Success = true,
                Message = "Category found",
                Data = category
            });
        }

        // POST: api/categories/create-or-get
        // Automatically creates category if not exists (used by product creation)
        [HttpPost("create-or-get")]
        public IActionResult CreateOrGet([FromBody] string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                categoryName = "Undefined";

            var category = _categoryService.GetOrCreateByName(categoryName.Trim());
            return Ok(new ApiResponse<Category>
            {
                Success = true,
                Message = "Category resolved",
                Data = category
            });
        }

        // PUT: api/categories/update/{id}
        [HttpPut("update/{id}")]
        public IActionResult Update(int id, [FromBody] Category updatedCategory)
        {
            if (id != updatedCategory.CategoryId)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Category ID mismatch"
                });
            }

            var result = _categoryService.Update(updatedCategory);
            if (result == null)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Category not found or update failed"
                });
            }

            return Ok(new ApiResponse<Category>
            {
                Success = true,
                Message = "Category updated successfully",
                Data = result
            });
        }
    }
}
