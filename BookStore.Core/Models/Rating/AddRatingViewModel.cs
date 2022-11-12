using System.ComponentModel.DataAnnotations;

namespace BookStore.Core.Models.Rating
{
	public class AddRatingViewModel
	{
        [Required]
        [Range(1, 5)]
        public int UserRating { get; set; }
    }
}
