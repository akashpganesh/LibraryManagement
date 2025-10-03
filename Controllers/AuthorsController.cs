using LibraryManagemant.Managers;
using LibraryManagemant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog.Context;

namespace LibraryManagemant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorManager _AuthorManager;

        public AuthorsController(IAuthorManager AuthorManager)
        {
            _AuthorManager = AuthorManager;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddAuthor([FromBody] AuthorRequest request)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                try
                {
                    await _AuthorManager.AddAuthorAsync(request);
                    return Ok(new { Message = "Author added successfully", CorrelationId = correlationId });
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(new { Message = ex.Message, CorrelationId = correlationId });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        Message = "An error occurred while adding the Author",
                        Details = ex.Message,
                        CorrelationId = correlationId
                    });
                }
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] AuthorRequest request)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                try
                {
                    await _AuthorManager.UpdateAuthorAsync(id, request);
                    return Ok(new { Message = "Author updated successfully", CorrelationId = correlationId });
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(new { Message = ex.Message, CorrelationId = correlationId });
                }
                catch (Exception ex) when (ex.Message.Contains("Author not found"))
                {
                    return NotFound(new { Message = ex.Message, CorrelationId = correlationId });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        Message = "An error occurred while updating the Author",
                        Details = ex.Message,
                        CorrelationId = correlationId
                    });
                }
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                try
                {
                    await _AuthorManager.DeleteAuthorAsync(id);
                    return Ok(new { Message = "Author deleted successfully", CorrelationId = correlationId });
                }
                catch (Exception ex) when (ex.Message.Contains("Author not found"))
                {
                    return NotFound(new { Message = ex.Message, CorrelationId = correlationId });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        Message = "An error occurred while deleting the Author",
                        Details = ex.Message,
                        CorrelationId = correlationId
                    });
                }
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllAuthors()
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                try
                {
                    var Authors = await _AuthorManager.GetAllAuthorsAsync();
                    return Ok(new
                    {
                        Message = "Authors retrieved successfully",
                        Data = Authors,
                        CorrelationId = correlationId
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        Message = "An error occurred while retrieving Authors",
                        Details = ex.Message,
                        CorrelationId = correlationId
                    });
                }
            }
        }
    }
}
