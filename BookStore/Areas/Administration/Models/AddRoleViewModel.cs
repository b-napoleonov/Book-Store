using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;
using static BookStore.Common.GlobalConstants;
using static BookStore.Common.GlobalExceptions;

namespace BookStore.Areas.Administration.Models
{
	public class AddRoleViewModel
	{
		[Required]
		[StringLength(RoleNameMaxLength, MinimumLength = RoleNameMinLength, ErrorMessage = StringFieldsErrorMessage)]
		public string RoleName { get; set; } = null!;
	}
}
