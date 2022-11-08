﻿using BookStore.Core.Contracts;
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

            var categoryBook = new CategoryBook()
            {
                Book = book,
                CategoryId = model.CategoryId,
            };

            book.Categories.Add(categoryBook);

            var warehouseBook = new WarehouseBook()
            {
                Book = book,
                WarehouseId = model.WarehouseId
            };

            book.WarehouseBooks.Add(warehouseBook);

            await bookRepository.AddAsync(book);
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
                .ThenInclude(b => b.Category)
                .Include(b => b.Reviews)
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
                Categories = book.Categories.Select(c => c.Category.Name).ToList(),
                Reviews = book.Reviews.Select(r => r.UserReview)
            };
        }

        public async Task<IEnumerable<AllBooksViewModel>> GetBooksByCategoryAsync(string categoryName)
        {
            var books = await bookRepository
                .AllAsNoTracking()
                .Include(b => b.Categories)
                .ThenInclude(cb => cb.Category)
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Where(b => b.Categories.Select(cb => cb.Category.Name == categoryName).FirstOrDefault())
                .ToListAsync();

            var models = new List<AllBooksViewModel>();

            foreach (var book in books)
            {
                var model = new AllBooksViewModel()
                {
                    Id = book.Id,
                    Title = book.Title,
                    ImageUrl = book.ImageUrl,
                    Author = book.Author.Name,
                    Price = book.Price,
                };

                models.Add(model);
            }

            if (books.Count <= 0)
            {
                throw new ArgumentException("Category not found");
            }

            return models;
        }

        public async Task<IEnumerable<AllBooksViewModel>> GetBooksByAuthorAsync(string authorName)
        {
            var books = await bookRepository
                .AllAsNoTracking()
                .Include(b => b.Categories)
                .ThenInclude(cb => cb.Category)
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Where(b => b.Author.Name == authorName)
                .ToListAsync();

            var models = new List<AllBooksViewModel>();

            foreach (var book in books)
            {
                var model = new AllBooksViewModel()
                {
                    Id = book.Id,
                    Title = book.Title,
                    ImageUrl = book.ImageUrl,
                    Author = book.Author.Name,
                    Price = book.Price,
                };

                models.Add(model);
            }

            if (books.Count <= 0)
            {
                throw new ArgumentException("Author not found");
            }

            return models;
        }

        public async Task<IEnumerable<AllBooksViewModel>> GetBooksByPublisherAsync(string publisherName)
        {
            var books = await bookRepository
                .AllAsNoTracking()
                .Include(b => b.Categories)
                .ThenInclude(cb => cb.Category)
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Where(b => b.Publisher.Name == publisherName)
                .ToListAsync();

            var models = new List<AllBooksViewModel>();

            foreach (var book in books)
            {
                var model = new AllBooksViewModel()
                {
                    Id = book.Id,
                    Title = book.Title,
                    ImageUrl = book.ImageUrl,
                    Author = book.Author.Name,
                    Price = book.Price,
                };

                models.Add(model);
            }

            if (books.Count <= 0)
            {
                throw new ArgumentException("Publisher not found");
            }

            return models;
        }

        public async Task<Book> GetBookByIdAsync(Guid bookId)
        {
            var book = await bookRepository
                .AllAsNoTracking()
                .Where(b => b.Id == bookId)
                .FirstOrDefaultAsync();

            if (book == null)
            {
                throw new ArgumentException("Invalid Book Id");
            }

            return book;
        }
    }
}
