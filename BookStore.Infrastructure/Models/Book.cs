using System.ComponentModel.DataAnnotations;

namespace BookStore.Infrastructure.Models
{
    public class Book
    {
        public Book()
        {
            this.Id = Guid.NewGuid();
            this.WarehouseBooks = new HashSet<WarehouseBook>();
            this.ShoppingBasketBooks = new HashSet<ShoppingBasketBook>();
            this.AuthorBooks = new HashSet<AuthorBook>();
        }

        public Guid Id { get; set; }

        [Required]
        public string ISBN { get; set; }

        [Required]
        public string Title { get; set; }

        public int Year { get; set; }

        public decimal Price { get; set; }

        [Required]
        public string PublisherName { get; set; }

        public int PublisherId { get; set; }

        public Publisher Publisher { get; set; }

        public ICollection<AuthorBook> AuthorBooks { get; set; }

        public ICollection<WarehouseBook> WarehouseBooks { get; set; }

        public ICollection<ShoppingBasketBook> ShoppingBasketBooks { get; set; }
    }
}
