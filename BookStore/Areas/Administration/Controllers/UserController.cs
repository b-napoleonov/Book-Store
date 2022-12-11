using BookStore.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IUserService = BookStore.Core.Contracts.IUserService;

namespace BookStore.Areas.Administration.Controllers
{
    public class UserController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly Core.Contracts.Admin.IUserService adminUserService;
        private readonly IUserService userService;

        public UserController(
            UserManager<ApplicationUser> _userManager,
            Core.Contracts.Admin.IUserService _adminUserService,
            IUserService _userService)
        {
            userManager = _userManager;
            adminUserService = _adminUserService;
            userService = _userService;
        }

        public static string UserControllerName => nameof(UserController).Replace("Controller", string.Empty);

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var model = await adminUserService.All();

            foreach (var user in model)
            {
                var currentUser = await userService.GetUserByIdAsync(user.UserId);
                var userRoles = await userManager.GetRolesAsync(currentUser);

                user.RoleNames = userRoles;
            }

            return View(model);
        }
    }
}
