using BookStore.Infrastructure.Common.SoftDeleteBaseClass;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Infrastructure.Models
{
    public class Order : IDeletableEntity
    {
        public Order()
        {
            this.Id = Guid.NewGuid();
            this.BookOrders = new HashSet<BookOrder>();
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        public string CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public ApplicationUser Customer { get; set; }

        public DateTime OrderDate { get; set; }

        [Required]
        public string OrderStatus { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public ICollection<BookOrder> BookOrders { get; set; }
    }
}
