using BookStore.Core.Constants;
using BookStore.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookStore.Core.Extensions;
using BookStore.Common;

namespace BookStore.Controllers
{
    public class BookController : BaseController
    {
        private const string IndexView = "All";
        private const string UserId = "UserId";
        private const string IndexViewTitle = "All Books";

        private readonly IBookService bookService;

        public BookController(IBookService _bookService)
        {
            bookService = _bookService;
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
        public async Task<IActionResult> Details(Guid bookId, string information)
        {
            try
            {
                string userId = GetCurrentUserId();

                ViewData[UserId] = userId;

                var model = await bookService.GetBookAsync(bookId);
                model.Reviews = await bookService.GetBookReviewsAsync(bookId);
                model.Ratings = await bookService.GetBookRatingDetailsAsync(bookId);

                if (information != model.GetInformation())
                {
                    TempData[MessageConstant.ErrorMessage] = GlobalExceptions.FailedToAccessHouseDetails;

                    return RedirectToAction(nameof(Index));
                }

                return View(model);
            }
            catch (ArgumentException ae)
            {
                TempData[MessageConstant.ErrorMessage] = ae.Message;

                return RedirectToAction(nameof(Index));
            }
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
