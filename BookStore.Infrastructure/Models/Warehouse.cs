using BookStore.Infrastructure.Common.SoftDeleteBaseClass;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Infrastructure.Models
{
    public class Warehouse : IDeletableEntity
    {
        public Warehouse()
        {
            this.WarehousesBooks = new HashSet<WarehouseBook>();
        }

        public int Id { get; set; }

        [Required]
        [MaxLength(70)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string Address { get; set; } = null!;

        [Required]
        [MaxLength(15)]
        public string Phone { get; set; } = null!;

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public ICollection<WarehouseBook> WarehousesBooks { get; set; }
    }
}
