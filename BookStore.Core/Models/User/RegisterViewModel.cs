using System.ComponentModel.DataAnnotations;
using static BookStore.Core.Constants.UserConstants;

namespace BookStore.Core.Models.User
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(UserNameMaxLenght, MinimumLength = UserNameMinLenght, ErrorMessage = "Your {0} must be between {2} and {1} characters.")]
        public string UserName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(EmailMaxLenght, MinimumLength = EmailMinLenght, ErrorMessage = "Your {0} must be between {2} and {1} characters.")]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(PasswordMaxLenght, MinimumLength = PasswordMinLenght, ErrorMessage = "Your {0} must be between {2} and {1} characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Compare(nameof(Password))]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = null!;
    }
}
