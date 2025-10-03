using LibraryManagemant.Models;

namespace LibraryManagemant.Managers
{
    public interface IAuthorManager
    {
        Task AddAuthorAsync(AuthorRequest request);
        Task UpdateAuthorAsync(int id, AuthorRequest request);
        Task DeleteAuthorAsync(int id);
        Task<IEnumerable<Author>> GetAllAuthorsAsync();
    }
}
