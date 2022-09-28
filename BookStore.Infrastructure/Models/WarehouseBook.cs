namespace BookStore.Infrastructure.Models
{
    public class WarehouseBook
    {
        public Guid BookId { get; set; }

        public Book Book { get; set; }

        public int WarehouseId { get; set; }

        public Warehouse Warehouse { get; set; }

        public int Count { get; set; }
    }
}
