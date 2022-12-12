using BookStore.Common;
using BookStore.Core.Constants;
using BookStore.Core.Contracts;
using BookStore.Core.Extensions;
using BookStore.Core.Models.Rating;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    public class RatingController : BaseController
    {
        private const string BookId = "BookId";

        private readonly IRatingService ratingService;

        public RatingController(IRatingService _ratingService)
        {
            ratingService = _ratingService;
        }

        [HttpGet]
        public IActionResult Add(Guid bookId)
        {
            var model = new AddRatingViewModel();

            ViewData[BookId] = bookId;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddRatingViewModel model, Guid bookId)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                string userId = GetCurrentUserId();

                await ratingService.AddRating(model, bookId, userId);

                return RedirectToAction(nameof(BookController.Index), BookController.BookControllerName);
                //return RedirectToAction(nameof(BookController.Details), BookController.BookControllerName,
                //    new { bookId = bookId, information = model.GetInformation() });
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, GlobalExceptions.Exception);

                return View(model);
            }
        }
    }
}
