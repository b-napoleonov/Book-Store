namespace BookStore.Infrastructure.Models
{
    public class PublisherBook
    {
        public Guid BookId { get; set; }

        public Book Book { get; set; }

        public int PublisherId { get; set; }

        public Publisher Publisher { get; set; }
    }
}
