using LibraryManagemant.Models;
using LibraryManagemant.Repositories;

namespace LibraryManagemant.Managers
{
    public class BookManager : IBookManager
    {
        private readonly IBookRepository _bookRepo;
        private readonly ILogger<BookManager> _logger;

        public BookManager(IBookRepository bookRepo, ILogger<BookManager> logger)
        {
            _bookRepo = bookRepo;
            _logger = logger;
        }

        public async Task AddBookAsync(AddBookRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Title))
                    throw new ArgumentException("Book title is required");
                if (string.IsNullOrWhiteSpace(request.ISBN))
                    throw new ArgumentException("ISBN is required");
                if (request.AuthorId <= 0)
                    throw new ArgumentException("Valid AuthorId is required");
                if (request.CategoryId <= 0)
                    throw new ArgumentException("Valid CategoryId is required");
                if (request.TotalCopies <= 0)
                    request.TotalCopies = 1;

                await _bookRepo.AddBook(request);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation failed while adding book: {Title}, ISBN: {ISBN}", request.Title, request.ISBN);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding book: {Title}, ISBN: {ISBN}", request.Title, request.ISBN);
                throw new Exception($"Error occurred while adding the book: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<BookResponse>> GetAllBooksAsync()
        {
            try
            {
                return await _bookRepo.GetAllBooks();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all books");
                throw new Exception($"Error retrieving books: {ex.Message}", ex);
            }
        }

        public async Task UpdateBookAsync(int bookId, UpdateBookRequest request)
        {
            try
            {
                if (bookId <= 0)
                    throw new ArgumentException("Valid BookId is required");

                await _bookRepo.UpdateBook(bookId, request);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, $"Validation failed while updating book {bookId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while updating book {bookId}");
                throw new Exception($"Error occurred while updating book {bookId}: {ex.Message}", ex);
            }
        }

        public async Task AddStockAsync(int bookId, int quantity)
        {
            if (bookId <= 0)
                throw new ArgumentException("Valid BookId is required");

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0");

            try
            {
                await _bookRepo.AddStockAsync(bookId, quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while adding stock for book {bookId}");
                throw;
            }
        }

        public async Task DeleteBookAsync(int bookId)
        {
            if (bookId <= 0)
                throw new ArgumentException("Valid BookId is required");

            try
            {
                await _bookRepo.DeleteBook(bookId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while deleting book {bookId}");
                throw;
            }
        }

        public async Task<BookResponse> GetBookByIdAsync(int bookId)
        {
            if (bookId <= 0)
                throw new ArgumentException("Valid BookId is required");

            try
            {
                return await _bookRepo.GetBookById(bookId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching book with ID {bookId}");
                throw;
            }
        }
    }
}
