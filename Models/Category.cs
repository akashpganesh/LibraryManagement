namespace LibraryManagemant.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }

    public class CategoryRequest
    {
        public string Name { get; set; } = null!;
    }
}
