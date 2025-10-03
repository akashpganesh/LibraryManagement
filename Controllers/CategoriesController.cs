using LibraryManagemant.Managers;
using LibraryManagemant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog.Context;

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
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                try
                {
                    await _categoryManager.AddCategoryAsync(request);
                    return Ok(new { Message = "Category added successfully", CorrelationId = correlationId });
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(new { Message = ex.Message, CorrelationId = correlationId });
                }
                catch (Exception ex) when (ex.Message.Contains("Category already exists"))
                {
                    return Conflict(new { Message = ex.Message, CorrelationId = correlationId }); // HTTP 409
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        Message = "An error occurred while adding the category",
                        Details = ex.Message,
                        CorrelationId = correlationId
                    });
                }
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryRequest request)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                try
                {
                    await _categoryManager.UpdateCategoryAsync(id, request);
                    return Ok(new { Message = "Category updated successfully", CorrelationId = correlationId });
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(new { Message = ex.Message, CorrelationId = correlationId });
                }
                catch (Exception ex) when (ex.Message.Contains("Category not found"))
                {
                    return NotFound(new { Message = ex.Message, CorrelationId = correlationId });
                }
                catch (Exception ex) when (ex.Message.Contains("Category already exists"))
                {
                    return Conflict(new { Message = ex.Message, CorrelationId = correlationId });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        Message = "An error occurred while updating the category",
                        Details = ex.Message,
                        CorrelationId = correlationId
                    });
                }
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                try
                {
                    await _categoryManager.DeleteCategoryAsync(id);
                    return Ok(new { Message = "Category deleted successfully", CorrelationId = correlationId });
                }
                catch (Exception ex) when (ex.Message.Contains("Category not found"))
                {
                    return NotFound(new { Message = ex.Message, CorrelationId = correlationId });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        Message = "An error occurred while deleting the category",
                        Details = ex.Message,
                        CorrelationId = correlationId
                    });
                }
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllCategories()
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                try
                {
                    var categories = await _categoryManager.GetAllCategoriesAsync();
                    return Ok(new
                    {
                        Message = "Categories retrieved successfully",
                        Data = categories,
                        CorrelationId = correlationId
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        Message = "An error occurred while retrieving categories",
                        Details = ex.Message,
                        CorrelationId = correlationId
                    });
                }
            }
        }
    }
}
