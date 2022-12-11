namespace BookStore.Areas.Administration.Models
{
	public class AssignRoleViewModel
	{
        public string UserId { get; set; } = null!;

        public string[] RoleNames { get; set; } = null!;
    }
}
