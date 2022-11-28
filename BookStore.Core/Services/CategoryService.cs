using BookStore.Common;
using BookStore.Core.Contracts;
using BookStore.Core.Models.Book;
using BookStore.Core.Models.Category;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookStore.Core.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IDeletableEntityRepository<Category> categoryRepository;
        private readonly ILogger<CategoryService> logger;

        public CategoryService(
            IDeletableEntityRepository<Category> _categoryRepository,
            ILogger<CategoryService> _logger)
        {
            categoryRepository = _categoryRepository;
            logger = _logger;
        }

        public async Task AddCategoryAsync(AddCategoryViewModel model)
        {
            var category = new Category
            {
                Name = model.Name,
            };

            try
            {
                await categoryRepository.AddAsync(category);
                await categoryRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(AddCategoryAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToSave, ex);
            }

        }

        public async Task<IEnumerable<BookCategoryViewModel>> GetAllCategoriesAsync()
        {
            var categories = new List<BookCategoryViewModel>();

            try
            {
                categories = await categoryRepository
                .AllAsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new BookCategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(nameof(GetAllCategoriesAsync), ex);

                throw new ApplicationException(GlobalExceptions.DatabaseFailedToFetch, ex);
            }

            return categories;
        }
    }
}
