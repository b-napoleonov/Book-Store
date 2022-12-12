namespace BookStore.Core.Models.Book
{
    /// <summary>
    /// Data for the book and its publisher
    /// </summary>
    public class BookPublisherViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
    }
}
