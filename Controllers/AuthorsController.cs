using LibraryManagemant.Managers;
using LibraryManagemant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            try
            {
                await _AuthorManager.AddAuthorAsync(request);

                return Ok(new { Message = "Author added successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while adding the Author",
                    Details = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] AuthorRequest request)
        {
            try
            {
                await _AuthorManager.UpdateAuthorAsync(id, request);
                return Ok(new { Message = "Author updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex) when (ex.Message.Contains("Author not found"))
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while updating the Author",
                    Details = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            try
            {
                await _AuthorManager.DeleteAuthorAsync(id);
                return Ok(new { Message = "Author deleted successfully" });
            }
            catch (Exception ex) when (ex.Message.Contains("Author not found"))
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while deleting the Author",
                    Details = ex.Message
                });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllAuthors()
        {
            try
            {
                var Authors = await _AuthorManager.GetAllAuthorsAsync();

                return Ok(new
                {
                    Message = "Authors retrieved successfully",
                    Data = Authors
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while retrieving Authors",
                    Details = ex.Message
                });
            }
        }
    }
}