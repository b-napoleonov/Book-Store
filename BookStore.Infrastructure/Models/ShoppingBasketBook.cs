namespace BookStore.Infrastructure.Models
{
    public class ShoppingBasketBook
    {
        public Guid BookId { get; set; }

        public Book Book { get; set; }

        public int ShoppingBasketId { get; set; }

        public ShoppingBasket ShoppingBasket { get; set; }
    }
}
