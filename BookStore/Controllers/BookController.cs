using BookStore.Core.Contracts;
using BookStore.Core.Models.Book;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class BookController : BaseController
    {
        private readonly IBookService bookService;
        private readonly IAuthorService authorService;
        private readonly ICategoryService categoryService;
        private readonly IPublisherService publisherService;
        private readonly IWarehouseService warehouseService;

        public BookController(
            IBookService _bookService,
            IAuthorService _authorService,
            ICategoryService _categoryService,
            IPublisherService _publisherService,
            IWarehouseService _warehouseService)
        {
            bookService = _bookService;
            authorService = _authorService;
            categoryService = _categoryService;
            publisherService = _publisherService;
            warehouseService = _warehouseService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var model = await bookService.GetAllBooksAsync();

            return View("All", model);
        }

        [HttpGet]
        public async Task< IActionResult> Details(Guid bookId)
        {
            try
            {
                var model = await bookService.GetBookAsync(bookId);

                //ToDo: Add necessary css and JS
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
                Authors = await authorService.GetAllAuthorsAsync(),
                Categories = await categoryService.GetAllCategoriesAsync(),
                Publishers = await publisherService.GetAllPublishersAsync(),
                Warehouses = await warehouseService.GetAllWarehousesAsync(),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddBookViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await bookService.AddBookAsync(model);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Something went wrong");

                return View(model);
            }
        }
    }
}
