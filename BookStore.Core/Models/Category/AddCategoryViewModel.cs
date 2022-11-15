using BookStore.Common;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Core.Models.Category
{
    public class AddCategoryViewModel
	{
        [Required]
        [StringLength(GlobalConstants.CategoryNameMaxLength, MinimumLength = GlobalConstants.CategoryNameMinLength, ErrorMessage = GlobalExceptions.StringFieldsErrorMessage)]
        public string Name { get; set; } = null!;
    }
}
