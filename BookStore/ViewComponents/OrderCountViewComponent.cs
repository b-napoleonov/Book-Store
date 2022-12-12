using BookStore.Core.Contracts;
using BookStore.Infrastructure.Models;
using BookStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.ViewComponents
{
    /// <summary>
    /// View Component holding the logic for visualising user's order count in the layout
    /// </summary>
    public class OrderCountViewComponent : ViewComponent
    {
        private readonly IUserService userService;
        private readonly UserManager<ApplicationUser> userManager;

        public OrderCountViewComponent(
            IUserService _userService,
            UserManager<ApplicationUser> _userManager)
        {
            userService = _userService;
            userManager = _userManager;
        }

        /// <summary>
        /// Gets user's order count from the Db and fills OrderCountViewModel
        /// </summary>
        /// <returns>IViewComponentResult</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await userManager.GetUserAsync(this.HttpContext.User);

            var userId = user?.Id;

            var orderCount = await userService.GetOrdersCountAsync(userId);

            var model = new OrderCountViewModel
            {
                OrderCount = orderCount,
            };

            return View(model);
        }
    }
}
