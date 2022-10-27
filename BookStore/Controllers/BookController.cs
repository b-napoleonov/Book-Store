using BookStore.Core.Contracts;
using BookStore.Core.Models.Book;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class BookController : BaseController
    {
        private readonly IBookService bookService;

        public BookController(IBookService _bookService)
        {
            bookService = _bookService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var model = await bookService.GetAllBooksAsync();

            return View("All", model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Details(Guid bookId)
        {
            try
            {
                var model = bookService.GetBookAsync(bookId);

                return View(model);
            }
            catch (ArgumentException ae)
            {
                //TODO: Book is invalid. Handle the exception

                throw;
            }
        }

        //TODO: Implement administrator who can add books from here
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new AddBookViewModel()
            {
                Authors = await bookService.GetAllAuthorsAsync(),
                Publishers = await bookService.GetAllPublishersAsync(),
                Categories = await bookService.GetAllCategoriesAsync(),
                Warehouses = await bookService.GetAllWarehousesAsync(),
            };

            return View(model);
        }
    }
}
