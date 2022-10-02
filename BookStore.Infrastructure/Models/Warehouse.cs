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
        [StringLength(70)]
        public string Name { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [Required]
        [StringLength(15)]
        public string Phone { get; set; }

        public ICollection<WarehouseBook> WarehousesBooks { get; set; }
    }
}
