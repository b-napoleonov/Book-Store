using System.ComponentModel.DataAnnotations;

namespace BookStore.Infrastructure.Models
{
    public class Author
    {
        public Author()
        {
            this.Books = new HashSet<Book>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(70)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(70)]
        public string LastName { get; set; } = null!;

        public ICollection<Book> Books { get; set; }
    }
}
