﻿namespace BookStore.Core.Models.Book
{
    public class AllBooksViewModel
    {
        public Guid Id { get; set; }

        public string ImageUrl { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string Author { get; set; } = null!;

        public decimal Price { get; set; }
    }
}