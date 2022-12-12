using BookStore.Common;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Core.Models.Review
{
    /// <summary>
    /// Data for creating new review
    /// </summary>
    public class ReviewViewModel
	{
        private const string ReviewDisplayName = "Review";

        [Required]
        [StringLength(GlobalConstants.ReviewMaxLength, MinimumLength = GlobalConstants.ReviewMinLength, ErrorMessage = GlobalExceptions.StringFieldsErrorMessage)]
        [Display(Name = ReviewDisplayName)]
        public string UserReview { get; set; } = null!;
    }
}
