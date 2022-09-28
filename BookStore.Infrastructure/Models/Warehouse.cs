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
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Phone { get; set; }

        public ICollection<WarehouseBook> WarehousesBooks { get; set; }
    }
}
