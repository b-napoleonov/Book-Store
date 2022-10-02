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
        [StringLength(13)]
        public string ISBN { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        //TODO: Custom Attribute for present year
        public int Year { get; set; }

        public decimal Price { get; set; }

        public int PublisherId { get; set; }

        public Publisher Publisher { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public ICollection<AuthorBook> AuthorBooks { get; set; }

        public ICollection<WarehouseBook> WarehouseBooks { get; set; }

        public ICollection<ShoppingBasketBook> ShoppingBasketBooks { get; set; }
    }
}
