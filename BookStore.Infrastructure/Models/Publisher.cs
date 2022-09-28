using System.ComponentModel.DataAnnotations;

namespace BookStore.Infrastructure.Models
{
    public class Publisher
    {
        public Publisher()
        {
            this.Books = new HashSet<Book>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string URL { get; set; }

        public ICollection<Book> Books { get; set; }
    }
}
