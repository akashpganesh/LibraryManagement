namespace LibraryManagemant.Models
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }

    public class AuthorRequest
    {
        public string Name { get; set; } = null!;
    }
}
