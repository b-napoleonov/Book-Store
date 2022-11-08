using System.ComponentModel.DataAnnotations;

namespace BookStore.Core.Models.Review
{
	public class ReviewViewModel
	{
        //TODO: Add reviewId if necessary?
        [Required]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "{0} must be between {2} and {1} characters.")]
        public string UserReview { get; set; } = null!;
    }
}
