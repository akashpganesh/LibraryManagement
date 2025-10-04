using LibraryManagemant.Models;

namespace LibraryManagemant.Managers
{
    public interface IBookManager
    {
        Task AddBookAsync(AddBookRequest request);
        Task<IEnumerable<BookResponse>> GetAllBooksAsync();
        Task UpdateBookAsync(int bookId, UpdateBookRequest request);
        Task AddStockAsync(int bookId, int quantity);
        Task DeleteBookAsync(int bookId);
        Task<BookResponse> GetBookByIdAsync(int bookId);
        Task<IEnumerable<BookResponse>> SearchBooksAsync(string searchText);
        Task<IEnumerable<BookResponse>> FilterBooksAsync(int? authorId, int? categoryId);
    }
}