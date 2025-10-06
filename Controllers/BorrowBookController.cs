using LibraryManagemant.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagemant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BorrowBookController : ControllerBase
    {
        private readonly IBorrowBookManager _borrowBookManager;

        public BorrowBookController(IBorrowBookManager borrowBookManager)
        {
            _borrowBookManager = borrowBookManager;
        }

        [HttpPost("{bookId}")]
        [Authorize]
        public async Task<IActionResult> BorrowBook(int bookId)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";
            using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value);

                try
                {
                    await _borrowBookManager.BorrowBookAsync(bookId, userId);
                    return Ok(new { Message = "Book borrowed successfully.", CorrelationId = correlationId });
                }
                catch (Exception ex) when (ex.Message.Contains("Book not found"))
                {
                    return NotFound(new { Message = ex.Message, CorrelationId = correlationId });
                }
                catch (Exception ex) when (ex.Message.Contains("No copies available"))
                {
                    return BadRequest(new { Message = ex.Message, CorrelationId = correlationId });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { Message = "An error occurred while borrowing book", Details = ex.Message, CorrelationId = correlationId });
                }
            }
        }

        [HttpPatch("return/{borrowId}")]
        [Authorize]
        public async Task<IActionResult> ReturnBook(int borrowId)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";
            using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
            {
                try
                {
                    var userId = int.Parse(User.FindFirst("UserId")?.Value);
                    var role = User.FindFirst(ClaimTypes.Role)?.Value;

                    var fine = await _borrowBookManager.ReturnBookAsync(borrowId, userId, role);
                    return Ok(new { Message = "Book returned successfully.", FineAmount = fine, CorrelationId = correlationId });
                }
                catch (Exception ex) when (ex.Message.Contains("Not authorized"))
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new
                    {
                        Message = ex.Message,
                        CorrelationId = correlationId
                    });
                }
                catch (Exception ex) when (ex.Message.Contains("Borrow record not found"))
                {
                    return NotFound(new { Message = ex.Message, CorrelationId = correlationId });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { Message = "An error occurred while returning the book", Details = ex.Message, CorrelationId = correlationId });
                }
            }
        }

        [HttpGet("borrowed")]
        [Authorize]
        public async Task<IActionResult> GetBorrowedBooks()
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString();
            using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
            {
                try
                {
                    var currentUserId = int.Parse(User.FindFirst("UserId")?.Value);
                    var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

                    // If Admin => get all borrowed books, otherwise filter by logged-in user
                    int? userId = currentUserRole == "Admin" ? null : currentUserId;

                    var borrowedBooks = await _borrowBookManager.GetBorrowedBooksAsync(userId);

                    if (!borrowedBooks.Any())
                        return NotFound(new { Message = "No borrowed books found.", CorrelationId = correlationId });

                    return Ok(new
                    {
                        Message = "Borrowed books retrieved successfully.",
                        Data = borrowedBooks,
                        CorrelationId = correlationId
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        Message = "An error occurred while retrieving borrowed books.",
                        Details = ex.Message,
                        CorrelationId = correlationId
                    });
                }
            }
        }

        [HttpGet("borrowed/{borrowId}")]
        [Authorize]
        public async Task<IActionResult> GetBorrowedBookById(int borrowId)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString();
            using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
            {
                try
                {
                    if (borrowId <= 0)
                        return BadRequest(new { Message = "Invalid BorrowId", CorrelationId = correlationId });

                    var currentUserId = int.Parse(User.FindFirst("UserId")?.Value);
                    var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

                    var borrowedBook = await _borrowBookManager.GetBorrowedBookByIdAsync(borrowId);

                    if (borrowedBook == null)
                        return NotFound(new { Message = "Borrowed book not found.", CorrelationId = correlationId });

                    // Restrict access: Users can only view their own borrow records
                    if (currentUserRole != "Admin" && borrowedBook.UserId != currentUserId)
                        return Forbid();

                    return Ok(new
                    {
                        Message = "Borrowed book retrieved successfully.",
                        Data = borrowedBook,
                        CorrelationId = correlationId
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        Message = "An error occurred while retrieving borrowed book.",
                        Details = ex.Message,
                        CorrelationId = correlationId
                    });
                }
            }
        }

        [HttpGet("filter")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> FilterBorrowedBooks([FromQuery] int? userId, [FromQuery] int? bookId)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString();
            using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
            {
                try
                {
                    var borrowedBooks = await _borrowBookManager.FilterBorrowedBooksAsync(userId, bookId);

                    if (borrowedBooks == null || !borrowedBooks.Any())
                        return NotFound(new
                        {
                            Message = "No borrowed books found matching the filter criteria.",
                            CorrelationId = correlationId
                        });

                    return Ok(new
                    {
                        Message = "Borrowed books retrieved successfully.",
                        Data = borrowedBooks,
                        CorrelationId = correlationId
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        Message = "An error occurred while filtering borrowed books.",
                        Details = ex.Message,
                        CorrelationId = correlationId
                    });
                }
            }
        }
    }
}
