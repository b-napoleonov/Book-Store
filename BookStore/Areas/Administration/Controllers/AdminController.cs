using BookStore.BaseControllers;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Areas.Administration.Controllers
{
    public class AdminController : BaseController
    {
        public static string AdminControllerName => nameof(AdminController).Replace("Controller", string.Empty);

        public IActionResult Index()
        {
            return View();
        }
    }
}
