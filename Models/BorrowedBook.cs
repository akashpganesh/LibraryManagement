namespace LibraryManagemant.Models
{
    public class BorrowedBook
    {
        public int BorrowId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public string AuthorName { get; set; }
        public string CategoryName { get; set; }
        public DateTime BorrowedAt { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnedAt { get; set; }
        public string Status { get; set; }
        public decimal FineAmount { get; set; }
    }
}
