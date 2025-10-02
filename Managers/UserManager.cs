using LibraryManagemant.Models;
using LibraryManagemant.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibraryManagemant.Managers
{
    public class UserManager : IUserManager
    {
        private readonly IUserRepository _userRepo;
        private readonly IConfiguration _config;
        private readonly ILogger<UserManager> _logger;

        public UserManager(IUserRepository userRepo, IConfiguration config, ILogger<UserManager> logger)
        {
            _userRepo = userRepo;
            _config = config;
            _logger = logger;
        }

        public async Task RegisterUser(string fullName, string email, string phone, string password)
        {
            try
            {
                await _userRepo.RegisterUser(fullName, email, phone, password, "Member");
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Email already exists"))
            {
                // Preserve business exception so controller can return 400
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while registering user {Email}", email);
                throw new Exception("An error occurred while registering the user. Please try again later.");
            }
        }

        public async Task<string> LoginUser(string email, string password)
        {
            try
            {
                var user = await _userRepo.LoginUser(email, password);
                if (user == null)
                    throw new UnauthorizedAccessException("Invalid email or password");

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim("UserId", user.UserId.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:ExpiresInMinutes"])),
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Failed login attempt for {Email}", email);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", email);
                throw new Exception("An error occurred while logging in. Please try again later.");
            }
        }

        public async Task<IEnumerable<UserDto>> GetAllUsers()
        {
            try
            {
                var users = await _userRepo.GetAllUsers();

                return users.Select(u => new UserDto
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    Role = u.Role,
                    Status = u.Status,
                    MembershipDate = u.MembershipDate
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all users");
                throw new Exception("An error occurred while fetching users. Please try again later.");
            }
        }

        public async Task<UserDto> GetUserById(int userId)
        {
            try
            {
                var user = await _userRepo.GetUserById(userId);

                if (user == null)
                    return null;

                return new UserDto
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role = user.Role,
                    Status = user.Status,
                    MembershipDate = user.MembershipDate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user with ID {UserId}", userId);
                throw new Exception("An error occurred while fetching the user. Please try again later.");
            }
        }

        public async Task UpdateUserProfile(int userId, UpdateUserRequest request)
        {
            try
            {
                if (request.FullName == null && request.Email == null && request.Phone == null)
                    throw new ArgumentException("At least one field must be provided to update");

                await _userRepo.UpdateUserProfile(userId, request.FullName, request.Email, request.Phone);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while updating user profile {UserId}", userId);
                throw;
            }
            catch (Exception ex) when (ex.Message.Contains("Email already exist"))
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile {UserId}", userId);
                throw new Exception("An error occurred while updating the user profile. Please try again later.");
            }
        }

        public async Task ChangePassword(int userId, ChangePasswordRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.OldPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
                    throw new ArgumentException("Old password and new password must be provided");

                await _userRepo.ChangePassword(userId, request.OldPassword, request.NewPassword);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while changing password for {UserId}", userId);
                throw;
            }
            catch (Exception ex) when (ex.Message.Contains("Old Password is Incorrect"))
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {UserId}", userId);
                throw new Exception("An error occurred while changing the password. Please try again later.");
            }
        }

        public async Task DeleteUser(int userId)
        {
            try
            {
                await _userRepo.DeleteUser(userId);
            }
            catch (Exception ex) when (ex.Message.Contains("User not found"))
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID {UserId}", userId);
                throw new Exception("An error occurred while deleting the user. Please try again later.");
            }
        }
    }
}
