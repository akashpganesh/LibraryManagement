using LibraryManagemant.Managers;
using LibraryManagemant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog.Context;

namespace LibraryManagemant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookManager _bookManager;

        public BooksController(IBookManager bookManager)
        {
            _bookManager = bookManager;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddBook([FromBody] AddBookRequest request)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                try
                {
                    await _bookManager.AddBookAsync(request);
                    return Ok(new { 
                        Message = "Book added successfully",
                        CorrelationId = correlationId
                    });
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(new { 
                        Message = ex.Message,
                        CorrelationId = correlationId
                    });
                }
                catch (Exception ex) when (ex.Message.Contains("ISBN"))
                {
                    return Conflict(new { 
                        Message = ex.Message,
                        CorrelationId = correlationId
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        Message = "An error occurred while adding the book",
                        Details = ex.Message,
                        CorrelationId = correlationId
                    });
                }
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllBooks()
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                try
                {
                    var books = await _bookManager.GetAllBooksAsync();
                    return Ok(new
                    {
                        Message = "Books retrieved successfully",
                        Data = books,
                        CorrelationId = correlationId
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        Message = "An error occurred while retrieving books",
                        Details = ex.Message,
                        CorrelationId = correlationId
                    });
                }
            }
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] UpdateBookRequest request)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                if (request == null)
                    return BadRequest(new { Message = "Invalid book update request", CorrelationId = correlationId });

                try
                {
                    await _bookManager.UpdateBookAsync(id, request);
                    return Ok(new { Message = "Book updated successfully", CorrelationId = correlationId });
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Book not found"))
                        return NotFound(new { Message = ex.Message, CorrelationId = correlationId });

                    if (ex.Message.Contains("ISBN already exists"))
                        return Conflict(new { Message = ex.Message, CorrelationId = correlationId });

                    return StatusCode(500, new { Message = "Unexpected error", Details = ex.Message, CorrelationId = correlationId });
                }
            }
        }

        [HttpPost("{bookId}/add-stock")]
        public async Task<IActionResult> AddStock(int bookId, [FromBody] AddStockRequest request)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                if (request == null)
                    return BadRequest(new { Message = "Invalid request", CorrelationId = correlationId });

                try
                {
                    await _bookManager.AddStockAsync(bookId, request.Quantity);
                    return Ok(new { Message = "Stock added successfully", CorrelationId = correlationId });
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(new { Message = ex.Message, CorrelationId = correlationId });
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Book not found"))
                        return NotFound(new { Message = ex.Message, CorrelationId = correlationId });

                    if (ex.Message.Contains("Quantity must be greater than 0"))
                        return BadRequest(new { Message = ex.Message, CorrelationId = correlationId });

                    return StatusCode(500, new { Message = "Unexpected error", Details = ex.Message, CorrelationId = correlationId });
                }
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                if (id <= 0)
                    return BadRequest(new { Message = "Invalid BookId", CorrelationId = correlationId });

                try
                {
                    await _bookManager.DeleteBookAsync(id);
                    return Ok(new { Message = "Book deleted successfully", CorrelationId = correlationId });
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Book not found"))
                        return NotFound(new { Message = ex.Message, CorrelationId = correlationId });

                    return StatusCode(500, new { Message = "Unexpected error", Details = ex.Message, CorrelationId = correlationId });
                }
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetBookById(int id)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                if (id <= 0)
                    return BadRequest(new { Message = "Invalid BookId", CorrelationId = correlationId });

                try
                {
                    var book = await _bookManager.GetBookByIdAsync(id);

                    return Ok(new { Message = "Book retrieved successfully", Data = book, CorrelationId = correlationId });
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Book not found"))
                        return NotFound(new { Message = ex.Message, CorrelationId = correlationId });

                    return StatusCode(500, new { Message = "Unexpected error", Details = ex.Message, CorrelationId = correlationId });
                }
            }
        }

        [HttpGet("search")]
        [Authorize]
        public async Task<IActionResult> SearchBooks([FromQuery] string? searchText)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                try
                {
                    var books = await _bookManager.SearchBooksAsync(searchText);

                    if (books == null || !books.Any())
                        return NotFound(new { Message = "No books found matching the search criteria.", CorrelationId = correlationId });

                    return Ok(new { Message = "Books retrieved successfully", Data = books, CorrelationId = correlationId });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        Message = "An error occurred while searching books",
                        Details = ex.Message,
                        CorrelationId = correlationId
                    });
                }
            }
        }

        [HttpGet("filter")]
        [Authorize] // Both Admin & Users can filter
        public async Task<IActionResult> FilterBooks([FromQuery] int? authorId, [FromQuery] int? categoryId)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";
            using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
            {
                try
                {
                    var books = await _bookManager.FilterBooksAsync(authorId, categoryId);

                    if (books == null || !books.Any())
                        return NotFound(new { Message = "No books found for the selected filters.", CorrelationId = correlationId });

                    return Ok(new { Message = "Books retrieved successfully", Data = books, CorrelationId = correlationId });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        Message = "An error occurred while filtering books",
                        Details = ex.Message,
                        CorrelationId = correlationId
                    });
                }
            }
        }

    }
}
