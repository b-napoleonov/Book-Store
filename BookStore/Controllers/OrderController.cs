using BookStore.Core.Constants;
using BookStore.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IOrderService orderService;

        public OrderController(IOrderService _orderService)
        {
            orderService = _orderService;
        }

        public async Task<IActionResult> Order(Guid bookId)
        {
            try
            {
                if (await orderService.CheckUserOrderAsync(GetCurrentUserId()))
                {
                    if (await orderService.CheckBookOrderAsync(bookId))
                    {
                        await orderService.AddCopiesToOrderAsync(bookId, GetCurrentUserId());
                    }
                    else
                    {
                        await orderService.AddNewBookToOrderAsync(bookId, GetCurrentUserId());
                    }

                    TempData[MessageConstant.SuccessMessage] = "Book added to your cart.";

                    return RedirectToAction("Index", "Book");
                }

                await orderService.AddNewOrderAsync(bookId, GetCurrentUserId());

                TempData[MessageConstant.SuccessMessage] = "Book added to your cart.";

                return RedirectToAction("Index", "Book");
            }
            catch (ArgumentException ae)
            {
                return View("Error404");
            }
        }

        public async Task<IActionResult> Remove(Guid bookId)
        {
            try
            {
                string userId = GetCurrentUserId();

                await orderService.RemoveUserOrdersAsync(bookId, userId);

                TempData[MessageConstant.WarningMessage] = "Order removed successfully.";

                return RedirectToAction(nameof(Cart));
            }
            catch (Exception)
            {
                return View("Error404");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            var model = await orderService.GetUserOrdersAsync(GetCurrentUserId());

            return View(model);
        }
    }
}
