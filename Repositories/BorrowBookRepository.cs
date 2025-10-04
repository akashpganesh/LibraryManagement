using Dapper;
using LibraryManagemant.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LibraryManagemant.Repositories
{
    public class BorrowBookRepository : IBorrowBookRepository
    {
        private readonly string _connectionString;
        public BorrowBookRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task BorrowBookAsync(int bookId, int userId)
        {
            try
            {
                using IDbConnection db = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();
                parameters.Add("@BookId", bookId);
                parameters.Add("@UserId", userId);

                await db.ExecuteAsync("sp_BorrowBook", parameters, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                throw new Exception($"Database error while borrowing book: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error while borrowing book: {ex.Message}", ex);
            }
        }

        public async Task<decimal> ReturnBookAsync(int borrowId)
        {
            try
            {
                using IDbConnection db = new SqlConnection(_connectionString);
                var parameters = new DynamicParameters();
                parameters.Add("@BorrowId", borrowId);
                parameters.Add("@Fine", dbType: DbType.Decimal, direction: ParameterDirection.Output);

                await db.ExecuteAsync("sp_ReturnBook", parameters, commandType: CommandType.StoredProcedure);

                return parameters.Get<decimal>("@Fine");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while returning book: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<BorrowedBook>> GetBorrowedBooksAsync(int? userId = null)
        {
            try
            {
                using IDbConnection db = new SqlConnection(_connectionString);

                var result = await db.QueryAsync<BorrowedBook>(
                    "sp_GetBorrowedBooks",
                    new { UserId = userId },
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving borrowed books: {ex.Message}", ex);
            }
        }

        public async Task<BorrowedBook?> GetBorrowedBookByIdAsync(int borrowId)
        {
            try
            {
                using IDbConnection db = new SqlConnection(_connectionString);

                return await db.QueryFirstOrDefaultAsync<BorrowedBook>(
                    "sp_GetBorrowedBookById",
                    new { BorrowId = borrowId },
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving borrowed book by ID: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<BorrowedBook>> FilterBorrowedBooksAsync(int? userId, int? bookId)
        {
            try
            {
                using IDbConnection db = new SqlConnection(_connectionString);

                return await db.QueryAsync<BorrowedBook>(
                    "sp_FilterBorrowedBooks",
                    new { UserId = userId, BookId = bookId },
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Database error while filtering borrowed books: {ex.Message}", ex);
            }
        }
    }
}
