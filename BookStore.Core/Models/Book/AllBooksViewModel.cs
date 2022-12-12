using BookStore.Core.Contracts;

namespace BookStore.Core.Models.Book
{
    /// <summary>
    /// Data needed for book index view
    /// </summary>
    public class AllBooksViewModel : IBookModel
    {
        public Guid Id { get; set; }

        public string ImageUrl { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string Author { get; set; } = null!;

        public decimal Price { get; set; }

        public double Rating { get; set; }
    }
}
