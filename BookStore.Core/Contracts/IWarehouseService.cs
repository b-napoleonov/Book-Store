using BookStore.Infrastructure.Models;

namespace BookStore.Core.Contracts
{
    public interface IWarehouseService
    {
        Task<IEnumerable<Warehouse>> GetAllWarehousesAsync();
    }
}
