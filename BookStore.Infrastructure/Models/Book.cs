﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Infrastructure.Models
{
    public class Book
    {
        public Book()
        {
            this.Id = Guid.NewGuid();
            this.WarehouseBooks = new HashSet<WarehouseBook>();
            this.ShoppingBasketBooks = new HashSet<ShoppingBasketBook>();
            this.Categories = new HashSet<CategoryBook>();
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(13)]
        public string ISBN { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = null!;

        //TODO: Custom Attribute for present year
        [Required]
        public int Year { get; set; }

        [Required]
        public decimal Price { get; set; }

        public int AuthorId { get; set; }

        [ForeignKey(nameof(AuthorId))]
        public Author Author { get; set; }

        public int PublisherId { get; set; }

        [ForeignKey(nameof(PublisherId))]
        public Publisher Publisher { get; set; }

        public ICollection<CategoryBook> Categories { get; set; }

        public ICollection<ShoppingBasketBook> ShoppingBasketBooks { get; set; }

        public ICollection<WarehouseBook> WarehouseBooks { get; set; }
    }
}
