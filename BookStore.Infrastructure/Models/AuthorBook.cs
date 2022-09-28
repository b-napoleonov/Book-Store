namespace BookStore.Infrastructure.Models
{
    public class AuthorBook
    {
        public Guid BookId { get; set; }

        public Book Book { get; set; }

        public int AuthorId { get; set; }

        public Author Author { get; set; }
    }
}
