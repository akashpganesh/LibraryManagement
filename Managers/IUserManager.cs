using LibraryManagemant.Models;

namespace LibraryManagemant.Managers
{
    public interface IUserManager
    {
        Task RegisterUser(string fullName, string email, string phone, string password);
        Task<string> LoginUser(string email, string password);
        Task<IEnumerable<UserDto>> GetAllUsers();
        Task<UserDto> GetUserById(int userId);
        Task UpdateUserProfile(int userId, UpdateUserRequest request);
        Task ChangePassword(int userId, ChangePasswordRequest request);
        Task DeleteUser(int userId);
    }
}
