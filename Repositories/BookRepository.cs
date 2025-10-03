using Dapper;
using LibraryManagemant.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LibraryManagemant.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly string _connectionString;
        public BookRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task AddBook(AddBookRequest request)
        {
            using IDbConnection db = new SqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@Title", request.Title);
            parameters.Add("@ISBN", request.ISBN);
            parameters.Add("@AuthorId", request.AuthorId);
            parameters.Add("@CategoryId", request.CategoryId);
            parameters.Add("@PublishedYear", request.PublishedYear);
            parameters.Add("@TotalCopies", request.TotalCopies);

            try
            {
                await db.ExecuteAsync("sp_AddBook", parameters, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex) when (ex.Number == 51010)
            {
                throw new Exception("Book with this ISBN already exists");
            }
            catch (Exception ex)
            {
                throw new Exception($"Database error: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<BookResponse>> GetAllBooks()
        {
            using IDbConnection db = new SqlConnection(_connectionString);

            try
            {
                var books = await db.QueryAsync<BookResponse>(
                    "sp_GetAllBooks",
                    commandType: CommandType.StoredProcedure);

                return books;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database error: {ex.Message}", ex);
            }
        }

        public async Task UpdateBook(int bookId, UpdateBookRequest request)
        {
            try
            {
                using IDbConnection db = new SqlConnection(_connectionString);

                await db.ExecuteAsync(
                    "sp_UpdateBook",
                    new
                    {
                        BookId = bookId,
                        request.Title,
                        request.ISBN,
                        request.AuthorId,
                        request.CategoryId,
                        request.PublishedYear
                    },
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (SqlException ex)
            {
                if (ex.Number == 51012)
                    throw new Exception("ISBN already exists");
                if (ex.Number == 51011)
                    throw new Exception("Book not found");

                throw new Exception($"Database error while updating book: {ex.Message}", ex);
            }
        }

        public async Task AddStockAsync(int bookId, int quantity)
        {
            try
            {
                using IDbConnection db = new SqlConnection(_connectionString);

                await db.ExecuteAsync(
                    "sp_AddBookStock",
                    new { BookId = bookId, Quantity = quantity },
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (SqlException ex)
            {
                if (ex.Number == 51013)
                    throw new Exception("Book not found");
                if (ex.Number == 51014)
                    throw new Exception("Quantity must be greater than 0");

                throw new Exception($"Database error while adding stock: {ex.Message}", ex);
            }
        }

        public async Task DeleteBook(int bookId)
        {
            try
            {
                using IDbConnection db = new SqlConnection(_connectionString);

                await db.ExecuteAsync(
                    "sp_DeleteBook",
                    new { BookId = bookId },
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (SqlException ex)
            {
                if (ex.Number == 51015)
                    throw new Exception("Book not found");

                throw new Exception($"Database error while deleting book: {ex.Message}", ex);
            }
        }

        public async Task<BookResponse> GetBookById(int bookId)
        {
            try
            {
                using IDbConnection db = new SqlConnection(_connectionString);

                var book = await db.QueryFirstOrDefaultAsync<BookResponse>(
                    "sp_GetBookById",
                    new { BookId = bookId },
                    commandType: CommandType.StoredProcedure
                );

                if (book == null)
                    throw new Exception("Book not found");

                return book;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 51016)
                    throw new Exception("Book not found");

                throw new Exception($"Database error while fetching book: {ex.Message}", ex);
            }
        }
    }
}