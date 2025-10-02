using LibraryManagemant.Managers;
using LibraryManagemant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            try
            {
                await _userManager.RegisterUser(request.FullName, request.Email, request.Phone, request.Password);
                return Ok(new
                {
                    Message = "User registered successfully"
                });
            }
            catch (ArgumentException ex) // Invalid inputs
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Email already exists"))
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while registering the user",
                    Details = ex.Message
                });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var token = await _userManager.LoginUser(request.Email, request.Password);
                return Ok(new
                {
                    Message = "Login successful",
                    Token = token
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    Message = "Invalid email or password"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while logging in. Please try again later.",
                    Details = ex.Message
                });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]  // Only Admin can access
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userManager.GetAllUsers();
                return Ok(new
                {
                    Message = "Users retrieved successfully",
                    Data = users
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while fetching users",
                    Details = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirst("UserId")?.Value);
                var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

                // Only Admin or the user themselves can access
                if (currentUserRole != "Admin" && currentUserId != id)
                {
                    return StatusCode(403, new { Message = "You are not authorized to access this user" });
                }

                var user = await _userManager.GetUserById(id);
                if (user == null)
                {
                    return NotFound(new { Message = "User not found" });
                }

                return Ok(new
                {
                    Message = "User fetched successfully",
                    Data = user
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while fetching the user",
                    Details = ex.Message
                });
            }
        }

        [HttpPatch]
        [Authorize]
        public async Task<IActionResult> UpdateUserProfile(int id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirst("UserId")?.Value);

                await _userManager.UpdateUserProfile(currentUserId, request);

                return Ok(new
                {
                    Message = "User profile updated successfully"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex) when (ex.Message.Contains("Email already exist"))
            {
                return Conflict(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while updating the user profile",
                    Details = ex.Message
                });
            }
        }

        [HttpPatch("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirst("UserId")?.Value);

                await _userManager.ChangePassword(currentUserId, request);

                return Ok(new { Message = "Password changed successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex) when (ex.Message.Contains("Old Password is Incorrect"))
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while changing the password",
                    Details = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only Admins can delete users
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _userManager.DeleteUser(id);

                return Ok(new
                {
                    Message = "User deleted successfully"
                });
            }
            catch (Exception ex) when (ex.Message.Contains("User not found"))
            {
                return NotFound(new { Message = ex.Message }); // HTTP 404
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while deleting the user",
                    Details = ex.Message
                });
            }
        }
    }
}