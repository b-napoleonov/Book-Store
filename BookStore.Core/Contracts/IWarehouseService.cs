using BookStore.Core.Models.Book;

namespace BookStore.Core.Contracts
{
    public interface IWarehouseService
    {
        Task<IEnumerable<BookWarehouseViewModel>> GetAllWarehousesAsync();
    }
}
