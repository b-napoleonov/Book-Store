using BookStore.Core.Contracts;
using BookStore.Core.Models.Book;
using BookStore.Core.Models.Category;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Core.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IDeletableEntityRepository<Category> categoryRepository;

        public CategoryService(IDeletableEntityRepository<Category> _categoryRepository)
        {
            categoryRepository = _categoryRepository;
        }

        public async Task AddCategoryAsync(AddCategoryViewModel model)
        {
            var category = new Category
            {
                Name = model.Name,
            };

            await categoryRepository.AddAsync(category);
            await categoryRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<BookCategoryViewModel>> GetAllCategoriesAsync()
        {
            return await categoryRepository
                .AllAsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new BookCategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }
    }
}
