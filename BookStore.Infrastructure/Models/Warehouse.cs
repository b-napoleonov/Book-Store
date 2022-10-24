using System.ComponentModel.DataAnnotations;

namespace BookStore.Infrastructure.Models
{
    public class Warehouse
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

        public ICollection<WarehouseBook> WarehousesBooks { get; set; }
    }
}
