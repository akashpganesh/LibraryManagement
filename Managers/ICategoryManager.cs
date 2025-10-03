using LibraryManagemant.Models;

namespace LibraryManagemant.Managers
{
    public interface ICategoryManager
    {
        Task AddCategoryAsync(CategoryRequest request);
        Task UpdateCategoryAsync(int id, CategoryRequest request);
        Task DeleteCategoryAsync(int id);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
    }
}
