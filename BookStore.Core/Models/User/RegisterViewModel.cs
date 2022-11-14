using LearnFast.Common;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Core.Models.User
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(GlobalConstants.UserNameMaxLenght, MinimumLength = GlobalConstants.UserNameMinLenght, ErrorMessage = GlobalExceptions.StringFieldsErrorMessage)]
        public string UserName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(GlobalConstants.EmailMaxLenght, MinimumLength = GlobalConstants.EmailMinLenght, ErrorMessage = GlobalExceptions.StringFieldsErrorMessage)]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(GlobalConstants.PasswordMaxLenght, MinimumLength = GlobalConstants.PasswordMinLenght, ErrorMessage = GlobalExceptions.StringFieldsErrorMessage)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Compare(nameof(Password))]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = null!;
    }
}
