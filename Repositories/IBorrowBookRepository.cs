using LibraryManagemant.Models;

namespace LibraryManagemant.Repositories
{
    public interface IBorrowBookRepository
    {
        Task BorrowBookAsync(int bookId, int userId);
        Task<decimal> ReturnBookAsync(int borrowId);
        Task<IEnumerable<BorrowedBook>> GetBorrowedBooksAsync(int? userId = null);
        Task<BorrowedBook?> GetBorrowedBookByIdAsync(int borrowId);
        Task<IEnumerable<BorrowedBook>> FilterBorrowedBooksAsync(int? userId, int? bookId);
    }
}