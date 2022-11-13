namespace BookStore.Core.Models.User
{
    public class UserProfileViewModel
    {
        public string UserName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? FirstName { get; set; } = null!;

        public string? LastName { get; set; } = null!;
    }
}
