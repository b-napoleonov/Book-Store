using BookStore.Infrastructure.Common.SoftDelete;

namespace BookStore.Infrastructure.Models
{
    public class ShoppingBasket : IDeletableEntity
    {
        public ShoppingBasket()
        {
            this.ShoppingBasketBooks = new HashSet<ShoppingBasketBook>();
        }

        public int Id { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        //TODO: Connect to User Entity

        public ICollection<ShoppingBasketBook> ShoppingBasketBooks { get; set; }
    }
}
