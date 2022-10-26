using BookStore.Core.Contracts;
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
        public async Task<IActionResult> Index()
        {
            var model = await bookService.GetAllBooksAsync();

            return View(model);
        }
    }
}
