using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Core.Models.User
{
    /// <summary>
    /// Data for Login given user
    /// </summary>
    public class LoginViewModel
    {
        public LoginViewModel()
        {
            this.ExternalLogins = new List<AuthenticationScheme>();
        }

        [Required]
        public string UserName { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
    }
}
