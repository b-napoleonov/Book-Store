namespace BookStore.Core.Models.Book
{
    /// <summary>
    /// Data for the book and its author
    /// </summary>
    public class BookAuthorViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
    }
}
