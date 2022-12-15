namespace BookStore.Core.Models.User
{
    /// <summary>
    /// Data for the user profile
    /// </summary>
    public class UserProfileViewModel
    {
        public string UserName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Phone { get; set; }

        public bool IsEmailConfirmed { get; set; }
    }
}
