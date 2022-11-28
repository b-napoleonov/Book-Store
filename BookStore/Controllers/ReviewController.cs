using BookStore.Common;
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
        private const string BookId = "BookId";
        private const string ReviewId = "ReviewId";

        private readonly IReviewService reviewService;

        public ReviewController(IReviewService _reviewService)
        {
            reviewService = _reviewService;
        }

        [HttpGet]
        public IActionResult Add(Guid bookId)
        {
            var model = new ReviewViewModel();

            ViewData[BookId] = bookId;

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

                TempData[MessageConstant.SuccessMessage] = GlobalConstants.ReviewAddedSuccessfully;

                return RedirectToAction(nameof(BookController.Details), BookController.BookControllerName, new {bookId = bookId });
            }
            catch (Exception)
            {
                TempData[MessageConstant.ErrorMessage] = GlobalExceptions.Exception;

                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int reviewId)
        {
            try
            {
                Review review = await reviewService.GetReviewByIdAsync(reviewId);

                ReviewViewModel model = new ReviewViewModel
                {
                    UserReview = review.UserReview
                };

                ViewData[ReviewId] = reviewId;

                return View(model);
            }
            catch (ArgumentException ae)
            {
                TempData[MessageConstant.ErrorMessage] = GlobalExceptions.Exception;

                return RedirectToAction(nameof(BookController.Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ReviewViewModel model, int reviewId)
        {
            try
            {
                string userId = GetCurrentUserId();

                await reviewService.UpdateReviewAsync(model, reviewId, userId);

                TempData[MessageConstant.SuccessMessage] = GlobalConstants.ReviewUpdatedSuccessfully;

                return RedirectToAction(nameof(BookController.Index), BookController.BookControllerName);
            }
            catch (Exception)
            {
                TempData[MessageConstant.ErrorMessage] = GlobalExceptions.Exception;

                return View(model);
            }
        }

        public async Task<IActionResult> Delete(int reviewId)
        {
            try
            {
                string userId = GetCurrentUserId();

                await reviewService.DeleteReviewAsync(reviewId, userId);

                TempData[MessageConstant.SuccessMessage] = GlobalConstants.ReviewDeletedSuccessfully;

                return RedirectToAction(nameof(BookController.Index), BookController.BookControllerName);
            }
            catch (ArgumentException ae)
            {
                TempData[MessageConstant.ErrorMessage] = GlobalExceptions.Exception;

                return RedirectToAction(nameof(BookController.Index));
            }
        }
    }
}
