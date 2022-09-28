using System.ComponentModel.DataAnnotations;

namespace BookStore.Infrastructure.Models
{
    public class Book
    {
        public Book()
        {
            this.Id = Guid.NewGuid();
            this.PublishersBooks = new HashSet<PublisherBook>();
            this.WarehousesBooks = new HashSet<WarehouseBook>();
            this.ShoppingBasketBooks = new HashSet<ShoppingBasketBook>();
        }

        public Guid Id { get; set; }

        [Required]
        public string ISBN { get; set; }

        [Required]
        public string Title { get; set; }

        public int Year { get; set; }

        public decimal Price { get; set; }

        [Required]
        public string AuthorName { get; set; }

        [Required]
        public string PublisherName { get; set; }

        public ICollection<PublisherBook> PublishersBooks { get; set; }

        public ICollection<WarehouseBook> WarehousesBooks { get; set; }

        public ICollection<ShoppingBasketBook> ShoppingBasketBooks { get; set; }
    }
}
