using BookStore.Areas.Administration.Controllers;
using BookStore.Core.Contracts;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static BookStore.Common.GlobalConstants;
using BaseController = BookStore.Controllers.BaseController;

namespace BookStore.BaseControllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBookService bookService;

        public HomeController(
            ILogger<HomeController> logger,
            IBookService _bookService)
        {
            _logger = logger;
            bookService = _bookService;
        }

        public static string HomeControllerName => nameof(HomeController).Replace("Controller", string.Empty);

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole(AdministratorRoleName))
            {
                return RedirectToAction(nameof(AdminController.Index), AdminController.AdminControllerName, new { area = AdministrationAreaName });
            }

            var model = await bookService.GetLastThreeBooksAsync();

            return View(model);
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Contact()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult NotFoundError()
        {
            return View("Error404");
        }
    }
}