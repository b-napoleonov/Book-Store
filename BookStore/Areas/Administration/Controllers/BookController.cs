using BookStore.Common;
using BookStore.Core.Constants;
using BookStore.Core.Contracts;
using BookStore.Core.Models.Book;
using Ganss.Xss;
using Microsoft.AspNetCore.Mvc;
using static BookStore.Common.GlobalConstants;

namespace BookStore.Areas.Administration.Controllers
{
    public class BookController : BaseController
    {
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
                var sanitizer = new HtmlSanitizer();

                model.ISBN = sanitizer.Sanitize(model.ISBN);
                model.Title = sanitizer.Sanitize(model.Title);
                model.Description = sanitizer.Sanitize(model.Description);
                model.ImageUrl = sanitizer.Sanitize(model.ImageUrl);

                await bookService.AddBookAsync(model);

                return RedirectToAction(nameof(BookStore.Controllers.BookController.Index), BookControllerName, new { area = "" });
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

            TempData[MessageConstant.SuccessMessage] = BookDeletedSuccessfully;

            return RedirectToAction(nameof(BookStore.Controllers.BookController.Index), BookControllerName, new { area = "" });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid bookId)
        {
            var model = await bookService.GetBookDataForEditAsync(bookId);

            model.Authors = await authorService.GetAllAuthorsAsync();
            model.Categories = await categoryService.GetAllCategoriesAsync();
            model.Publishers = await publisherService.GetAllPublishersAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditBookViewModel model, Guid bookId)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var sanitizer = new HtmlSanitizer();

            model.ISBN = sanitizer.Sanitize(model.ISBN);
            model.Title = sanitizer.Sanitize(model.Title);
            model.Description = sanitizer.Sanitize(model.Description);
            model.ImageUrl = sanitizer.Sanitize(model.ImageUrl);

            await bookService.EditBookAsync(model, bookId);

            TempData[MessageConstant.SuccessMessage] = BookEditedSuccessfully;

            return RedirectToAction(nameof(BookStore.Controllers.BookController.Index), BookControllerName, new { area = "" });
        }
    }
}
