using System.ComponentModel.DataAnnotations;

namespace BookStore.Core.Models.Rating
{
	public class RatingViewModel
	{
        [Required]
        [Range(1, 5)]
        public int UserRating { get; set; }
    }
}
