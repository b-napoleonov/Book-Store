using BookStore.Core.Contracts;

namespace BookStore.Core.Models.Book
{
    /// <summary>
    /// Data for the carousel on the home page
    /// </summary>
    public class HomeBookViewModel : IBookModel
    {
        public string Title { get; set; } = null!;

        public string ImageUrl { get; set; } = null!;
    }
}
