namespace BookStore.Core.Models.Admin
{
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
