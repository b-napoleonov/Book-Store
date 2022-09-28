namespace BookStore.Infrastructure.Models
{
    public class ShoppingBasket
    {
        public ShoppingBasket()
        {
            this.ShoppingBasketBooks = new HashSet<ShoppingBasketBook>();
        }

        public int Id { get; set; }

        //TODO: Connect to User Entity

        public ICollection<ShoppingBasketBook> ShoppingBasketBooks { get; set; }
    }
}
