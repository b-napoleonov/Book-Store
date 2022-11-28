using BookStore.Common;
using BookStore.Core.Constants;
using BookStore.Core.Contracts;
using BookStore.Core.Models.Book;
using Microsoft.AspNetCore.Mvc;

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


        //TODO: Implement new layout for this area
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

            TempData[MessageConstant.SuccessMessage] = GlobalConstants.BookDeletedSuccessfully;

            return RedirectToAction(nameof(Index));
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

            await bookService.EditBookAsync(model, bookId);

            TempData[MessageConstant.SuccessMessage] = GlobalConstants.BookEditedSuccessfully;

            return RedirectToAction(nameof(Index));
        }
    }
}
