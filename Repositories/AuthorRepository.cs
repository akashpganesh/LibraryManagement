using Dapper;
using LibraryManagemant.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LibraryManagemant.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly string _connectionString;
        public AuthorRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task AddAuthor(string name)
        {
            try
            {
                using IDbConnection db = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();
                parameters.Add("@Name", name);

                await db.ExecuteAsync("sp_AddAuthor", parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new Exception($"Database error: {ex.Message}", ex);
            }
        }

        public async Task UpdateAuthor(int AuthorId, string name)
        {
            try
            {
                using IDbConnection db = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();
                parameters.Add("@AuthorId", AuthorId);
                parameters.Add("@Name", name);

                await db.ExecuteAsync("sp_UpdateAuthor", parameters, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex) when (ex.Number == 51008)
            {
                throw new Exception("Author not found");
            }
            catch (Exception ex)
            {
                throw new Exception($"Database error: {ex.Message}", ex);
            }
        }

        public async Task DeleteAuthor(int AuthorId)
        {
            try
            {
                using IDbConnection db = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();
                parameters.Add("@AuthorId", AuthorId);

                await db.ExecuteAsync("sp_DeleteAuthor", parameters, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex) when (ex.Number == 51009)
            {
                throw new Exception("Author not found");
            }
            catch (Exception ex)
            {
                throw new Exception($"Database error: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Author>> GetAllAuthors()
        {
            try
            {
                using IDbConnection db = new SqlConnection(_connectionString);

                var Authors = await db.QueryAsync<Author>(
                    "sp_GetAllAuthors",
                    commandType: CommandType.StoredProcedure);

                return Authors;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database error: {ex.Message}", ex);
            }
        }
    }
}
