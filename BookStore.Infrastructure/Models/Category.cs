using System.ComponentModel.DataAnnotations;

namespace BookStore.Infrastructure.Models
{
    public class Category
    {
        public Category()
        {
            this.Books = new HashSet<CategoryBook>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        public ICollection<CategoryBook> Books { get; set; }
    }
}
