namespace BookStore.Core.Models.Order
{
	public class OrderViewModel
	{
		public Guid BookId { get; set; }

		public string Title { get; set; } = null!;

		public string Author { get; set; } = null!;

        public decimal Price { get; set; }

		public string ImageUrl { get; set; } = null!;

		public int Copies { get; set; }
    }
}
