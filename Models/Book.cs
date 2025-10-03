namespace LibraryManagemant.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; } = null!;
        public string ISBN { get; set; } = null!;
        public int AuthorId { get; set; }
        public int CategoryId { get; set; }
        public int? PublishedYear { get; set; }
        public int CopiesAvailable { get; set; } = 1;
        public int TotalCopies { get; set; } = 1;
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }

    public class AddBookRequest
    {
        public string Title { get; set; } = null!;
        public string ISBN { get; set; } = null!;
        public int AuthorId { get; set; }
        public int CategoryId { get; set; }
        public int? PublishedYear { get; set; }
        public int TotalCopies { get; set; } = 1;
    }

    public class BookResponse
    {
        public int BookId { get; set; }
        public string Title { get; set; } = null!;
        public string ISBN { get; set; } = null!;
        public int PublishedYear { get; set; }
        public int TotalCopies { get; set; }
        public int CopiesAvailable { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = null!;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
    }

    public class UpdateBookRequest
    {
        public string? Title { get; set; }
        public string? ISBN { get; set; }
        public int? AuthorId { get; set; }
        public int? CategoryId { get; set; }
        public int? PublishedYear { get; set; }
    }

    public class AddStockRequest
    {
        public int Quantity { get; set; }
    }
}
