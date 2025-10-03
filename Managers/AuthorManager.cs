using LibraryManagemant.Models;
using LibraryManagemant.Repositories;

namespace LibraryManagemant.Managers
{
    public class AuthorManager : IAuthorManager
    {
        private readonly IAuthorRepository _AuthorRepo;
        private readonly ILogger<AuthorManager> _logger;

        public AuthorManager(IAuthorRepository AuthorRepo, ILogger<AuthorManager> logger)
        {
            _AuthorRepo = AuthorRepo;
            _logger = logger;
        }

        public async Task AddAuthorAsync(AuthorRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    throw new ArgumentException("Author name is required");

                await _AuthorRepo.AddAuthor(request.Name);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation failed while adding Author");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding Author {AuthorName}", request.Name);
                throw new Exception($"Error occurred while adding the Author : {ex.Message}", ex);
            }
        }

        public async Task UpdateAuthorAsync(int id, AuthorRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    throw new ArgumentException("Author name is required");

                await _AuthorRepo.UpdateAuthor(id, request.Name);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation failed while updating Author {AuthorId}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating Author {AuthorId}", id);
                throw new Exception($"An error occurred while updating the Author : {ex.Message}", ex);
            }
        }

        public async Task DeleteAuthorAsync(int id)
        {
            try
            {
                await _AuthorRepo.DeleteAuthor(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting Author {AuthorId}", id);
                throw new Exception($"Error occurred while deleting the Author : {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Author>> GetAllAuthorsAsync()
        {
            try
            {
                return await _AuthorRepo.GetAllAuthors();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all Authors");
                throw new Exception($"Error occurred while fetching Authors : {ex.Message}", ex);
            }
        }
    }
}
