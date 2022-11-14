using System.ComponentModel.DataAnnotations;

namespace BookStore.Core.Models.Category
{
    public class AddCategoryViewModel
	{
        [Required]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "{0} must be between {1} and {2} characters.")]
        public string Name { get; set; } = null!;
    }
}
