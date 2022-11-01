using BookStore.Core.Contracts;
using BookStore.Core.Models.Book;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Core.Services
{
    public class BookService : IBookService
    {
        private readonly IDeletableEntityRepository<Book> bookRepository;

        public BookService(IDeletableEntityRepository<Book> _bookRepository)
        {
            bookRepository = _bookRepository;
        }

        public async Task AddBookAsync(AddBookViewModel model)
        {
            //TODO: Find better implementation
            var book = new Book()
            {
                ISBN = model.ISBN,
                Title = model.Title,
                Description = model.Description,
                Year = model.Year,
                Price = model.Price,
                Pages = model.Pages,
                ImageUrl = model.ImageUrl,
                AuthorId = model.AuthorId,
                PublisherId = model.PublisherId
            };

            await bookRepository.AddAsync(book);
            await bookRepository.SaveChangesAsync();

            var categories = new HashSet<CategoryBook>();

            foreach (var category in model.Categories)
            {
                categories.Add(new CategoryBook
                {
                    BookId = book.Id,
                    CategoryId = category.Id,
                });
            }

            var warehouses = new HashSet<WarehouseBook>();

            foreach (var warehouse in model.Warehouses)
            {
                warehouses.Add(new WarehouseBook
                {
                    BookId = book.Id,
                    WarehouseId = warehouse.Id,
                });
            }

            book.Categories = categories;
            book.WarehouseBooks = warehouses;

            await bookRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<AllBooksViewModel>> GetAllBooksAsync()
        {
            return await bookRepository.AllAsNoTracking()
                .Select(b => new AllBooksViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    ImageUrl = b.ImageUrl,
                    Author = b.Author.Name,
                    Price = b.Price,
                })
                .ToListAsync();
        }

        public async Task<DetailsBookViewModel> GetBookAsync(Guid bookId)
        {
            var book = await bookRepository
                .AllAsNoTracking()
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Categories)
                .Where(x => x.Id == bookId)
                .FirstOrDefaultAsync();

            if (book == null)
            {
                throw new ArgumentException("Invalid Book ID");
            }

            return new DetailsBookViewModel()
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                Year = book.Year,
                Price = book.Price,
                Pages = book.Pages,
                ImageUrl = book.ImageUrl,
                Author = book.Author.Name,
                Publisher = book.Publisher.Name,
                Categories = book.Categories.Select(c => c.Category.Name).ToList()
            };
        }
    }
}
