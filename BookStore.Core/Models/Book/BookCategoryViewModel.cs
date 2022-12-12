namespace BookStore.Core.Models.Book
{
    /// <summary>
    /// Data for the book and its category
    /// </summary>
    public class BookCategoryViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
    }
}
