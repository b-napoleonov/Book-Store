using BookStore.Core.Contracts;
using BookStore.Core.Models.Rating;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
	public class RatingController : BaseController
	{
		private readonly IRatingService ratingService;

		public RatingController(IRatingService _ratingService)
		{
			ratingService = _ratingService;
		}

		[HttpGet]
		public IActionResult Add(Guid bookId)
		{
			var model = new AddRatingViewModel();

            ViewData["BookId"] = bookId;

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

				return RedirectToAction("Details", "Book", new { bookId = bookId });
			}
			catch (Exception)
			{
                ModelState.AddModelError(string.Empty, "Something went wrong");

                return View(model);
            }
        }
    }
}
