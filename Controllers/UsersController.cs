using LibraryManagemant.Managers;
using LibraryManagemant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Serilog.Context;

namespace LibraryManagemant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserManager _userManager;

        public UsersController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                try
                {
                    await _userManager.RegisterUser(request.FullName, request.Email, request.Phone, request.Password);
                    return Ok(new { Message = "User registered successfully", CorrelationId = correlationId });
                }
                catch (ArgumentException ex) // Invalid inputs
                {
                    return BadRequest(new { Message = ex.Message, CorrelationId = correlationId });
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("Email already exists"))
                {
                    return Conflict(new { Message = ex.Message, CorrelationId = correlationId });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        Message = "An error occurred while registering the user",
                        Details = ex.Message,
                        CorrelationId = correlationId
                    });
                }
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                try
                {
                    var token = await _userManager.LoginUser(request.Email, request.Password);
                    return Ok(new { Message = "Login successful", Token = token, CorrelationId = correlationId });
                }
                catch (UnauthorizedAccessException)
                {
                    return Unauthorized(new { Message = "Invalid email or password", CorrelationId = correlationId });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        Message = "An error occurred while logging in. Please try again later.",
                        Details = ex.Message,
                        CorrelationId = correlationId
                    });
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]  // Only Admin can access
        public async Task<IActionResult> GetAllUsers()
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                try
                {
                    var users = await _userManager.GetAllUsers();
                    return Ok(new { Message = "Users retrieved successfully", Data = users, CorrelationId = correlationId });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        Message = "An error occurred while fetching users",
                        Details = ex.Message,
                        CorrelationId = correlationId
                    });
                }
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(int id)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                try
                {
                    var currentUserId = int.Parse(User.FindFirst("UserId")?.Value);
                    var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

                    if (currentUserRole != "Admin" && currentUserId != id)
                        return StatusCode(403, new { Message = "You are not authorized to access this user", CorrelationId = correlationId });

                    var user = await _userManager.GetUserById(id);
                    if (user == null)
                        return NotFound(new { Message = "User not found", CorrelationId = correlationId });

                    return Ok(new { Message = "User fetched successfully", Data = user, CorrelationId = correlationId });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        Message = "An error occurred while fetching the user",
                        Details = ex.Message,
                        CorrelationId = correlationId
                    });
                }
            }
        }

        [HttpPatch]
        [Authorize]
        public async Task<IActionResult> UpdateUserProfile(int id, [FromBody] UpdateUserRequest request)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                try
                {
                    var currentUserId = int.Parse(User.FindFirst("UserId")?.Value);
                    await _userManager.UpdateUserProfile(currentUserId, request);
                    return Ok(new { Message = "User profile updated successfully", CorrelationId = correlationId });
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(new { Message = ex.Message, CorrelationId = correlationId });
                }
                catch (Exception ex) when (ex.Message.Contains("Email already exist"))
                {
                    return Conflict(new { Message = ex.Message, CorrelationId = correlationId });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        Message = "An error occurred while updating the user profile",
                        Details = ex.Message,
                        CorrelationId = correlationId
                    });
                }
            }
        }

        [HttpPatch("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                try
                {
                    var currentUserId = int.Parse(User.FindFirst("UserId")?.Value);
                    await _userManager.ChangePassword(currentUserId, request);
                    return Ok(new { Message = "Password changed successfully", CorrelationId = correlationId });
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(new { Message = ex.Message, CorrelationId = correlationId });
                }
                catch (Exception ex) when (ex.Message.Contains("Old Password is Incorrect"))
                {
                    return Unauthorized(new { Message = ex.Message, CorrelationId = correlationId });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        Message = "An error occurred while changing the password",
                        Details = ex.Message,
                        CorrelationId = correlationId
                    });
                }
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only Admins can delete users
        public async Task<IActionResult> DeleteUser(int id)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                try
                {
                    await _userManager.DeleteUser(id);
                    return Ok(new { Message = "User deleted successfully", CorrelationId = correlationId });
                }
                catch (Exception ex) when (ex.Message.Contains("User not found"))
                {
                    return NotFound(new { Message = ex.Message, CorrelationId = correlationId });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        Message = "An error occurred while deleting the user",
                        Details = ex.Message,
                        CorrelationId = correlationId
                    });
                }
            }
        }
    }
}
