using BookStore.Core.Contracts;
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

        public async Task<IEnumerable<Warehouse>> GetAllWarehousesAsync()
        {
            return await warehouseRepository.AllAsNoTracking().ToListAsync();
        }
    }
}
