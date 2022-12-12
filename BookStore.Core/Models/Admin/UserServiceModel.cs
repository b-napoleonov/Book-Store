namespace BookStore.Core.Models.Admin
{
    /// <summary>
    /// Fills the data for all users to be visualised on admin view
    /// </summary>
    public class UserServiceModel
    {
        public UserServiceModel()
        {
            this.RoleNames = new HashSet<string>();
        }

        public string UserId { get; init; } = null!;

        public string Email { get; init; } = null!;

        public string? FirstName { get; init; }

        public string? LastName { get; init; }

        public string? PhoneNumber { get; init; }

        public IEnumerable<string> RoleNames { get; set; }
    }
}
