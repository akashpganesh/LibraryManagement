using LibraryManagemant.Models;
using LibraryManagemant.Repositories;

namespace LibraryManagemant.Managers
{
    public class CategoryManager : ICategoryManager
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly ILogger<CategoryManager> _logger;

        public CategoryManager(ICategoryRepository categoryRepo, ILogger<CategoryManager> logger)
        {
            _categoryRepo = categoryRepo;
            _logger = logger;
        }

        public async Task AddCategoryAsync(CategoryRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    throw new ArgumentException("Category name is required");

                await _categoryRepo.AddCategory(request.Name);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation failed while adding category");
                throw;
            }
            catch (Exception ex) when (ex.Message.Contains("Category already exists"))
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding category {CategoryName}", request.Name);
                throw new Exception("An error occurred while adding the category. Please try again later.");
            }
        }

        public async Task UpdateCategoryAsync(int id, CategoryRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    throw new ArgumentException("Category name is required");

                await _categoryRepo.UpdateCategory(id, request.Name);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation failed while updating category {CategoryId}", id);
                throw;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Category not found") || ex.Message.Contains("Category already exists"))
                {
                    throw;
                }
                else
                {
                    _logger.LogError(ex, "Error occurred while updating category {CategoryId}", id);
                    throw new Exception("An error occurred while updating the category. Please try again later.");
                }
            }
        }

        public async Task DeleteCategoryAsync(int id)
        {
            try
            {
                await _categoryRepo.DeleteCategory(id);
            }
            catch (Exception ex) when (ex.Message.Contains("Category not found"))
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting category {CategoryId}", id);
                throw new Exception("An error occurred while deleting the category. Please try again later.");
            }
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            try
            {
                return await _categoryRepo.GetAllCategories();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all categories");
                throw new Exception("An error occurred while fetching categories. Please try again later.");
            }
        }
    }
}
