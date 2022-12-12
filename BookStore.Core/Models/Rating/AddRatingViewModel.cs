using BookStore.Common;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Core.Models.Rating
{
    /// <summary>
    /// Data for adding new rating
    /// </summary>
    public class AddRatingViewModel
	{
        [Required]
        [Range(GlobalConstants.RatingMinRange, GlobalConstants.RatingMaxRange, ErrorMessage = GlobalExceptions.NumberFieldsErrorMessage)]
        public int UserRating { get; set; }
    }
}
