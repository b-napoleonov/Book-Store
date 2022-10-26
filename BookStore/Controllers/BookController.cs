using BookStore.Core.Contracts;
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
            catch (ArgumentException)
            {
                //TODO: Book is invalid. Handle the exception

                throw;
            }
        }
    }
}
