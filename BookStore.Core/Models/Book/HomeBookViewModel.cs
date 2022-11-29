using BookStore.Core.Contracts;

namespace BookStore.Core.Models.Book
{
    public class HomeBookViewModel : IBookModel
    {
        public string Title { get; set; } = null!;

        public string ImageUrl { get; set; } = null!;
    }
}
