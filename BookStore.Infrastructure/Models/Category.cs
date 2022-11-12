using BookStore.Infrastructure.Common.SoftDeleteBaseClass;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Infrastructure.Models
{
    public class Category :IDeletableEntity
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

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public ICollection<CategoryBook> Books { get; set; }
    }
}
