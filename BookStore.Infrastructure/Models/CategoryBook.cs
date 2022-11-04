using BookStore.Infrastructure.Common.SoftDeleteBaseClass;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Infrastructure.Models
{
    public class CategoryBook : IDeletableEntity
    {
        [Required]
        public Guid BookId { get; set; }

        [ForeignKey(nameof(BookId))]
        public Book Book { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
