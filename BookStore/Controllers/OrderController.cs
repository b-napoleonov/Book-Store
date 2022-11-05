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
			await orderService.AddOrderAsync(bookId, GetCurrentUserId());
			//TODO: Add order and refresh the page
			return View();
		}
	}
}
