using LibraryManagemant.Models;

namespace LibraryManagemant.Repositories
{
    public interface IBookRepository
    {
        Task AddBook(AddBookRequest request);
        Task<IEnumerable<BookResponse>> GetAllBooks();
        Task UpdateBook(int bookId, UpdateBookRequest request);
        Task AddStockAsync(int bookId, int quantity);
        Task DeleteBook(int bookId);
        Task<BookResponse> GetBookById(int bookId);
    }
}
