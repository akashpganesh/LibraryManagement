using LibraryManagemant.Models;

namespace LibraryManagemant.Repositories
{
    public interface ICategoryRepository
    {
        Task AddCategory(string name);
        Task UpdateCategory(int categoryId, string name);
        Task DeleteCategory(int categoryId);
        Task<IEnumerable<Category>> GetAllCategories();
    }
}
