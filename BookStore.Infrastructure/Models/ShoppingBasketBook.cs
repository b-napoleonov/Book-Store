﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Infrastructure.Models
{
    public class ShoppingBasketBook
    {
        public Guid BookId { get; set; }

        [ForeignKey(nameof(BookId))]
        public Book Book { get; set; }

        public int ShoppingBasketId { get; set; }

        [ForeignKey(nameof(ShoppingBasketId))]
        public ShoppingBasket ShoppingBasket { get; set; }

        [Range(1, 10)]
        public int Count { get; set; }
    }
}
