﻿namespace BookStore.Core.Models.Order
{
	public class OrderViewModel
	{
		public string Title { get; set; } = null!;

		public decimal Price { get; set; }

		public string ImageUrl { get; set; } = null!;

		public int Copies { get; set; }
    }
}