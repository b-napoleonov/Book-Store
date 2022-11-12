using BookStore.Core.Contracts;
using BookStore.Core.Models.Book;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Core.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IDeletableEntityRepository<Warehouse> warehouseRepository;

        public WarehouseService(IDeletableEntityRepository<Warehouse> _warehouseRepository)
        {
            warehouseRepository = _warehouseRepository;
        }

        public async Task<IEnumerable<BookWarehouseViewModel>> GetAllWarehousesAsync()
        {
            return await warehouseRepository
                .AllAsNoTracking()
                .OrderBy(w => w.Name)
                .Select(w => new BookWarehouseViewModel
                {
                    Id = w.Id,
                    Name = w.Name
                })
                .ToListAsync();
        }
    }
}
