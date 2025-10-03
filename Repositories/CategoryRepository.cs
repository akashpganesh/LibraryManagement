using Dapper;
using LibraryManagemant.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LibraryManagemant.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly string _connectionString;
        public CategoryRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task AddCategory(string name)
        {
            try
            {
                using IDbConnection db = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();
                parameters.Add("@Name", name);

                await db.ExecuteAsync("sp_AddCategory", parameters, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex) when (ex.Number == 51004)
            {
                throw new Exception("Category already exists");
            }
            catch (Exception ex)
            {
                throw new Exception($"Database error: {ex.Message}", ex);
            }
        }

        public async Task UpdateCategory(int categoryId, string name)
        {
            try
            {
                using IDbConnection db = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();
                parameters.Add("@CategoryId", categoryId);
                parameters.Add("@Name", name);

                await db.ExecuteAsync("sp_UpdateCategory", parameters, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex) when (ex.Number == 51005)
            {
                throw new Exception("Category not found");
            }
            catch (SqlException ex) when (ex.Number == 51006)
            {
                throw new Exception("Category already exists");
            }
            catch (Exception ex)
            {
                throw new Exception($"Database error: {ex.Message}", ex);
            }
        }

        public async Task DeleteCategory(int categoryId)
        {
            try
            {
                using IDbConnection db = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();
                parameters.Add("@CategoryId", categoryId);

                await db.ExecuteAsync("sp_DeleteCategory", parameters, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex) when (ex.Number == 51007)
            {
                throw new Exception("Category not found");
            }
            catch (Exception ex)
            {
                throw new Exception($"Database error: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            try
            {
                using IDbConnection db = new SqlConnection(_connectionString);

                var categories = await db.QueryAsync<Category>(
                    "sp_GetAllCategories",
                    commandType: CommandType.StoredProcedure);

                return categories;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database error: {ex.Message}", ex);
            }
        }
    }
}
