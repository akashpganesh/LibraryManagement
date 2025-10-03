using LibraryManagemant.Managers;
using LibraryManagemant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagemant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryManager _categoryManager;

        public CategoriesController(ICategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCategory([FromBody] CategoryRequest request)
        {
            try
            {
                await _categoryManager.AddCategoryAsync(request);

                return Ok(new { Message = "Category added successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex) when (ex.Message.Contains("Category already exists"))
            {
                return Conflict(new { Message = ex.Message }); // HTTP 409
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while adding the category",
                    Details = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryRequest request)
        {
            try
            {
                await _categoryManager.UpdateCategoryAsync(id, request);
                return Ok(new { Message = "Category updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex) when (ex.Message.Contains("Category not found"))
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex) when (ex.Message.Contains("Category already exists"))
            {
                return Conflict(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while updating the category",
                    Details = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _categoryManager.DeleteCategoryAsync(id);
                return Ok(new { Message = "Category deleted successfully" });
            }
            catch (Exception ex) when (ex.Message.Contains("Category not found"))
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while deleting the category",
                    Details = ex.Message
                });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _categoryManager.GetAllCategoriesAsync();

                return Ok(new
                {
                    Message = "Categories retrieved successfully",
                    Data = categories
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while retrieving categories",
                    Details = ex.Message
                });
            }
        }
    }
}
