using BookStore.Core.Constants;
using BookStore.Core.Contracts;
using BookStore.Core.Models.Book;
using LearnFast.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class BookController : BaseController
    {
        private const string IndexView = "All";
        private const string UserId = "UserId";
        private const string IndexViewTitle = "All Books";

        private readonly IBookService bookService;
        private readonly IAuthorService authorService;
        private readonly ICategoryService categoryService;
        private readonly IPublisherService publisherService;

        public BookController(
            IBookService _bookService,
            IAuthorService _authorService,
            ICategoryService _categoryService,
            IPublisherService _publisherService)
        {
            bookService = _bookService;
            authorService = _authorService;
            categoryService = _categoryService;
            publisherService = _publisherService;
        }

        public static string BookControllerName => nameof(BookController).Replace("Controller", string.Empty);

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var model = await bookService.GetAllBooksAsync();

            ViewBag.Title = IndexViewTitle;

            return View(IndexView, model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid bookId)
        {
            try
            {
                string userId = GetCurrentUserId();

                ViewData[UserId] = userId;

                var model = await bookService.GetBookAsync(bookId);
                model.Reviews = await bookService.GetBookReviewsAsync(bookId);
                model.Ratings = await bookService.GetBookRatingAsync(bookId);

                return View(model);
            }
            catch (ArgumentException ae)
            {
                TempData[MessageConstant.ErrorMessage] = ae.Message;

                return RedirectToAction(nameof(Index));
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
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddBookViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Authors = await authorService.GetAllAuthorsAsync();
                model.Categories = await categoryService.GetAllCategoriesAsync();
                model.Publishers = await publisherService.GetAllPublishersAsync();

                return View(model);
            }

            try
            {
                await bookService.AddBookAsync(model);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, GlobalExceptions.Exception);

                return View(model);
            }
        }

        public async Task<IActionResult> Remove(Guid bookId)
        {
            await bookService.RemoveBook(bookId);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Author(string authorName)
        {
            try
            {
                var model = await bookService.GetBooksByAuthorAsync(authorName);

                ViewBag.Title = authorName.ToUpper();

                return View(IndexView, model);
            }
            catch (ArgumentException ae)
            {
                TempData[MessageConstant.ErrorMessage] = ae.Message;

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Publisher(string publisherName)
        {
            try
            {
                var model = await bookService.GetBooksByPublisherAsync(publisherName);

                ViewBag.Title = publisherName.ToUpper();

                return View(IndexView, model);
            }
            catch (ArgumentException ae)
            {
                TempData[MessageConstant.ErrorMessage] = ae.Message;

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Category(string categoryName)
        {
            try
            {
                var model = await bookService.GetBooksByCategoryAsync(categoryName);

                ViewBag.Title = categoryName.ToUpper();

                return View(IndexView, model);
            }
            catch (ArgumentException ae)
            {
                TempData[MessageConstant.ErrorMessage] = ae.Message;

                return RedirectToAction(nameof(Index));
            }
        }
    }
}
