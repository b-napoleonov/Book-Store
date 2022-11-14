using LearnFast.Common;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Core.Models.Review
{
	public class ReviewViewModel
	{
        private const string ReviewDisplayName = "Review";

        [Required]
        [StringLength(GlobalConstants.ReviewMaxLength, MinimumLength = GlobalConstants.ReviewMinLength, ErrorMessage = GlobalExceptions.StringFieldsErrorMessage)]
        [Display(Name = ReviewDisplayName)]
        public string UserReview { get; set; } = null!;
    }
}
