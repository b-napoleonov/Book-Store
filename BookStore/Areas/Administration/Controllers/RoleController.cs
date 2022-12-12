using BookStore.Areas.Administration.Models;
using BookStore.Core.Constants;
using BookStore.Core.Contracts;
using BookStore.Infrastructure.Models;
using Ganss.Xss;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static BookStore.Common.GlobalConstants;

namespace BookStore.Areas.Administration.Controllers
{
    public class RoleController : BaseController
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserService userService;

        public RoleController(
            RoleManager<IdentityRole> _roleManager,
            UserManager<ApplicationUser> _userManager,
            IUserService _userService)
        {
            roleManager = _roleManager;
            userManager = _userManager;
            userService = _userService;
        }

        [HttpGet]
        public IActionResult Add()
        {
            var model = new AddRoleViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                if (!await roleManager.RoleExistsAsync(model.RoleName))
                {
                    var sanitizer = new HtmlSanitizer();

                    model.RoleName = sanitizer.Sanitize(model.RoleName);

                    await roleManager.CreateAsync(new IdentityRole()
                    {
                        Name = model.RoleName
                    });
                }

                return RedirectToAction(nameof(UserController.All), UserController.UserControllerName, new { area = AdministrationAreaName });
            }
            catch (ArgumentNullException an)
            {
                TempData[MessageConstant.ErrorMessage] = an.Message;

                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Assign(string userId)
        {
            var user = await userService.GetUserByIdAsync(userId);

            var model = new AssignRoleViewModel()
            {
                UserId = userId
            };

            ViewBag.RoleItems = roleManager.Roles
                .ToList()
                .Select(r => new SelectListItem()
                {
                    Text = r.Name,
                    Value = r.Name,
                    Selected = userManager.IsInRoleAsync(user, r.Name).Result
                }).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Assign(AssignRoleViewModel model)
        {
            var user = await userService.GetUserByIdAsync(model.UserId);
            var userRoles = await userManager.GetRolesAsync(user);
            await userManager.RemoveFromRolesAsync(user, userRoles);

            if (model.RoleNames?.Length > 0)
            {
                await userManager.AddToRolesAsync(user, model.RoleNames);
            }

            return RedirectToAction(nameof(UserController.All), UserController.UserControllerName, new { area = AdministrationAreaName });
        }
    }
}
