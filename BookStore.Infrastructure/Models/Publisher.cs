using System.ComponentModel.DataAnnotations;

namespace BookStore.Infrastructure.Models
{
    public class Publisher
    {
        public Publisher()
        {
            this.PublishersBooks = new HashSet<PublisherBook>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string URL { get; set; }

        public ICollection<PublisherBook> PublishersBooks { get; set; }
    }
}
