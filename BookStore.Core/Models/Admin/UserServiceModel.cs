namespace BookStore.Core.Models.Admin
{
    public class UserServiceModel
    {
        public string UserId { get; init; } = null!;

        public string Email { get; init; } = null!;

        public string? FirstName { get; init; }

        public string? LastName { get; init; }

        public string? PhoneNumber { get; init; }
    }
}
