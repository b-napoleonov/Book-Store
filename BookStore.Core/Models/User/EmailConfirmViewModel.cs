using System.ComponentModel.DataAnnotations;
using static BookStore.Common.GlobalConstants;
using static BookStore.Common.GlobalExceptions;

namespace BookStore.Core.Models.User
{
	/// <summary>
	/// View model for confirming user Email address
	/// </summary>
	public class EmailConfirmViewModel
	{
		[Required]
		[StringLength(UserTokenMaxLength, MinimumLength = UserTokenMinLength, ErrorMessage = InvalidUserToken)]
		public string UserCode { get; set; } = null!;
	}
}
