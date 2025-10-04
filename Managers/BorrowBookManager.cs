using LibraryManagemant.Models;
using LibraryManagemant.Repositories;

namespace LibraryManagemant.Managers
{
    public class BorrowBookManager : IBorrowBookManager
    {
        private readonly IBorrowBookRepository _borrowBookRepo;
        private readonly ILogger<BorrowBookManager> _logger;

        public BorrowBookManager(IBorrowBookRepository borrowBookRepo, ILogger<BorrowBookManager> logger)
        {
            _borrowBookRepo = borrowBookRepo;
            _logger = logger;
        }

        public async Task BorrowBookAsync(int bookId, int userId)
        {
            try
            {
                await _borrowBookRepo.BorrowBookAsync(bookId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error borrowing BookId={BookId} for UserId={UserId}", bookId, userId);
                throw;
            }
        }

        public async Task<decimal> ReturnBookAsync(int borrowId)
        {
            try
            {
                return await _borrowBookRepo.ReturnBookAsync(borrowId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error returning BorrowId={BorrowId}", borrowId);
                throw;
            }
        }

        public async Task<IEnumerable<BorrowedBook>> GetBorrowedBooksAsync(int? userId = null)
        {
            try
            {
                return await _borrowBookRepo.GetBorrowedBooksAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving borrowed books for UserId={UserId}", userId);
                throw;
            }
        }

        public async Task<BorrowedBook?> GetBorrowedBookByIdAsync(int borrowId)
        {
            try
            {
                return await _borrowBookRepo.GetBorrowedBookByIdAsync(borrowId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving borrowed book with BorrowId={BorrowId}", borrowId);
                throw;
            }
        }

        public async Task<IEnumerable<BorrowedBook>> FilterBorrowedBooksAsync(int? userId, int? bookId)
        {
            try
            {
                return await _borrowBookRepo.FilterBorrowedBooksAsync(userId, bookId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering borrowed books for UserId={UserId}, BookId={BookId}", userId, bookId);
                throw;
            }
        }
    }
}
