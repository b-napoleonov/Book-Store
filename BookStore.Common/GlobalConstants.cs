namespace BookStore.Common
{
    public static class GlobalConstants
    {
        public const string AdministrationAreaName = "Administration";

        public const string AdministratorRoleName = "Administrator";

        public const string BookOrderdSuccessfully = "Book added to your cart.";

        public const string OrderRemovedSuccessfully = "Order removed successfully.";

        public const string ReviewAddedSuccessfully = "Review added successfully.";

        public const string ReviewUpdatedSuccessfully = "Review updated successfully.";

        public const string ReviewDeletedSuccessfully = "Review deleted successfully.";

        public const string BookDeletedSuccessfully = "Book deleted successfully";

        public const string BookEditedSuccessfully = "Book edited successfully";

        public const int ISBNMaxLength = 13;
        public const int ISBNMinLength = 13;

        public const int TitleMaxLength = 100;
        public const int TitleMinLength = 2;

        public const int DescriptionMaxLength = 500;
        public const int DescriptionMinLength = 10;

        public const int CurrentYearValue = 1900;

        public const string PriceMinRange = "0.0";
        public const string PriceMaxRange = "500.0";

        public const int PagesMaxRange = 9999;
        public const int PagesMinRange = 10;

        public const int QuantityMaxRange = 999;
        public const int QuantityMinRange = 1;

        public const int CategoryNameMaxLength = 50;
        public const int CategoryNameMinLength = 5;

        public const int RatingMaxRange = 5;
        public const int RatingMinRange = 1;

        public const int ReviewMaxLength = 500;
        public const int ReviewMinLength = 10;

        public const int UserNameMaxLenght = 20;
        public const int UserNameMinLenght = 5;

        public const int EmailMaxLenght = 60;
        public const int EmailMinLenght = 10;

        public const int PasswordMaxLenght = 20;
        public const int PasswordMinLenght = 5;

        public const int UserFirstNameMaxLength = 50;

        public const int UserLastNameMaxLength = 50;

        public const int AuthorNameMaxLength = 70;

        public const int PublisherNameMaxLength = 100;
        public const int PublisherPhoneMaxLength = 15;
        public const int PublisherEmailMaxLength = 50;
        public const int PublisherURLMaxLength = 50;

        public const int RoleNameMaxLength = 20;
        public const int RoleNameMinLength = 3;
    }
}
