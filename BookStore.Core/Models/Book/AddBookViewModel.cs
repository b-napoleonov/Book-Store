﻿using BookStore.Core.CustomAttributes;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Core.Models.Book
{
    public class AddBookViewModel
    {
        public AddBookViewModel()
        {
            this.Authors = new List<BookAuthorViewModel>();
            this.Publishers = new List<BookPublisherViewModel>();
            this.Categories = new List<BookCategoryViewModel>();
        }

        [Required]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "{0} must be between {1} and {2} characters.")]
        public string ISBN { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "{0} must be between {1} and {2} characters.")]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "{0} must be between {1} and {2} characters.")]
        public string Description { get; set; } = null!;

        [Required]
        [CurrentYearValue(1900)]
        public int Year { get; set; }

        [Required]
        [Range(typeof(decimal), "0.0", "500.0", ConvertValueInInvariantCulture = true, ErrorMessage = "{0} must be between {1} and {2}.")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, 9999, ErrorMessage = "{0} must be between {1} and {2}.")]
        public int Pages { get; set; }

        [Required]
        [Range(1, 999, ErrorMessage = "{0} must be between {1} and {2}.")]
        public int Quantity { get; set; }

        [Required]
        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; } = null!;

        [Display(Name = "Author")]
        public int AuthorId { get; set; }

        public IEnumerable<BookAuthorViewModel> Authors { get; set; }

        [Display(Name = "Publisher")]
        public int PublisherId { get; set; }

        public IEnumerable<BookPublisherViewModel> Publishers { get; set; }

        [Display(Name = "Category")]
        public IEnumerable<int> CategoryIds { get; set; }

        public IEnumerable<BookCategoryViewModel> Categories { get; set; }
    }
}
