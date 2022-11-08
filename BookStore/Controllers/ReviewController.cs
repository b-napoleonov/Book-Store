using BookStore.Core.Constants;
using BookStore.Core.Contracts;
using BookStore.Core.Models.Review;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
	public class ReviewController : BaseController
	{
		private readonly IReviewService reviewService;

		public ReviewController(IReviewService _reviewService)
		{
			reviewService = _reviewService;
		}

		[HttpGet]
		public IActionResult Add(Guid bookId)
		{
			var model = new AddReviewViewModel();

			ViewData["BookId"] = bookId;

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Add(AddReviewViewModel model, Guid bookId)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			try
			{
                await reviewService.AddReviewAsync(model, bookId, GetCurrentUserId());

                TempData[MessageConstant.SuccessMessage] = "Review added successfully.";

                return RedirectToAction("Details", "Book", bookId);
            }
			catch (Exception)
			{
                ModelState.AddModelError(string.Empty, "Something went wrong");

                return View(model);
            }
        }
	}
}
