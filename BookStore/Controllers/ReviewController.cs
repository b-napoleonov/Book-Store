using BookStore.Core.Constants;
using BookStore.Core.Contracts;
using BookStore.Core.Models.Review;
using BookStore.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
            var model = new ReviewViewModel();

            ViewData["BookId"] = bookId;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ReviewViewModel model, Guid bookId)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await reviewService.AddReviewAsync(model, bookId, GetCurrentUserId());

                TempData[MessageConstant.SuccessMessage] = "Review added successfully.";

                return RedirectToAction("Details", "Book", new {bookId = bookId });
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Something went wrong");

                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int reviewId)
        {
            try
            {
                Review review = await reviewService.GetReviewByIdAsync(reviewId);

                string currentUserId = GetCurrentUserId();

                ReviewViewModel model = new ReviewViewModel
                {
                    UserReview = review.UserReview
                };

                ViewData["ReviewId"] = reviewId;

                return View(model);
            }
            catch (Exception)
            {
                return View("Error404");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ReviewViewModel model, int reviewId)
        {
            try
            {
                string userId = GetCurrentUserId();

                await reviewService.UpdateReviewAsync(model, reviewId, userId);

                TempData[MessageConstant.SuccessMessage] = "Review updated successfully.";

                return RedirectToAction("Index", "Book");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Something went wrong");

                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int reviewId)
        {
            try
            {
                string userId = GetCurrentUserId();

                await reviewService.DeleteReviewAsync(reviewId, userId);

                TempData[MessageConstant.SuccessMessage] = "Review deleted successfully.";

                return RedirectToAction("Index", "Book");
            }
            catch (Exception)
            {
                return View("Error404");
            }
        }
    }
}
