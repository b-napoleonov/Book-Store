namespace BookStore.Models
{
    /// <summary>
    /// Error View Model of the app
    /// </summary>
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}