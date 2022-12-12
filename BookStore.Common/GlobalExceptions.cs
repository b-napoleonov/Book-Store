namespace BookStore.Common
{
    /// <summary>
    /// Global exception messages of the app
    /// </summary>
    public static class GlobalExceptions
    {
        public const string Exception = "Something went wrong.";

        public const string DatabaseFailedToFetch = "Failed to fetch info from DB";

        public const string DatabaseFailedToSave = "Database failed to save data";

        public const string InvalidLogin = "Invalid Login!";

        public const string InvalidBookId = "Invalid Book Id.";

        public const string CategoryNotFound = "Category not found.";

        public const string AuthrorNotFound = "Author not found.";

        public const string PublisherNotFound = "Publisher not found.";

        public const string InsufficientQuantity = "Insufficient quantity.";

        public const string InvalidUser = "Invalid User.";

        public const string InvalidOrder = "Invalid order.";

        public const string InvalidRating = "Invalid Rating.";

        public const string InvalidReviewId = "Invalid Review Id.";

        public const string NotReviewOwner = "You are not the review owner.";

        public const string StringFieldsErrorMessage = "{0} must be between {1} and {2} characters.";

        public const string NumberFieldsErrorMessage = "{0} must be between {1} and {2}.";

        public const string FailedToAccessHouseDetails = "Failed to acces house details!";
    }
}
