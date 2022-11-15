using BookStore.Common;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Core.Models.Rating
{
	public class AddRatingViewModel
	{
        [Required]
        [Range(GlobalConstants.RatingMinRange, GlobalConstants.RatingMaxRange, ErrorMessage = GlobalExceptions.NumberFieldsErrorMessage)]
        public int UserRating { get; set; }
    }
}
