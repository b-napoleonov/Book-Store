using BookStore.Common;
using BookStore.Core.Contracts;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Core.Models.Rating
{
	public class AddRatingViewModel : IBookModel
	{
        [Required]
        [Range(GlobalConstants.RatingMinRange, GlobalConstants.RatingMaxRange, ErrorMessage = GlobalExceptions.NumberFieldsErrorMessage)]
        public int UserRating { get; set; }

        public string Title { get; init; } = null!;
    }
}
