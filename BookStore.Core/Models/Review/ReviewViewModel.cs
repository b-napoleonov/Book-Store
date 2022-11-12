using System.ComponentModel.DataAnnotations;

namespace BookStore.Core.Models.Review
{
	public class ReviewViewModel
	{
        [Required]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "{0} must be between {2} and {1} characters.")]
        [Display(Name = "Review")]
        public string UserReview { get; set; } = null!;
    }
}
