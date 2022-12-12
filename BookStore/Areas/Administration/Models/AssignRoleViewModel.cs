namespace BookStore.Areas.Administration.Models
{
    /// <summary>
    /// View Model with required user data to assign him a role
    /// </summary>
	public class AssignRoleViewModel
	{
        public string UserId { get; set; } = null!;

        public string[] RoleNames { get; set; } = null!;
    }
}
