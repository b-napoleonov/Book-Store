using BookStore.Common;
using BookStore.Core.Contracts;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Core.Models.Review
{
	public class ReviewViewModel : IBookModel
	{
        private const string ReviewDisplayName = "Review";

        [Required]
        [StringLength(GlobalConstants.ReviewMaxLength, MinimumLength = GlobalConstants.ReviewMinLength, ErrorMessage = GlobalExceptions.StringFieldsErrorMessage)]
        [Display(Name = ReviewDisplayName)]
        public string UserReview { get; set; } = null!;

        public string Title { get; init; } = null!;
    }
}
