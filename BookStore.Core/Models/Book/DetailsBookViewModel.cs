namespace BookStore.Core.Models.Book
{
    public class DetailsBookViewModel
    {
        public Guid Id { get; set; }

        public string ISBN { get; set; } = null!;


        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public int Year { get; set; }

        public decimal Price { get; set; }

        public int Pages { get; set; }

        public string ImageUrl { get; set; } = null!;

        public string Author { get; set; } = null!;

        public string Publisher { get; set; } = null!;

        public IEnumerable<string> Categories { get; set; } = null!;
    }
}
