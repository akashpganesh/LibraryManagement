using LibraryManagemant.Models;

namespace LibraryManagemant.Repositories
{
    public interface IAuthorRepository
    {
        Task AddAuthor(string name);
        Task UpdateAuthor(int AuthorId, string name);
        Task DeleteAuthor(int AuthorId);
        Task<IEnumerable<Author>> GetAllAuthors();
    }
}
