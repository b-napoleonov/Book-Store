using BookStore.Common;
using BookStore.Core.Contracts;
using BookStore.Core.Models.Category;
using Microsoft.AspNetCore.Mvc;
using static BookStore.Common.GlobalConstants;

namespace BookStore.Areas.Administration.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService _categoryService)
        {
            categoryService = _categoryService;
        }

        [HttpGet]
        public IActionResult Add()
        {
            var model = new AddCategoryViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await categoryService.AddCategoryAsync(model);

                //TODO: Think of more meaningful redirect
                return RedirectToAction(nameof(BookController.Add), BookController.BookControllerName, new { area = AdministrationAreaName });
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, GlobalExceptions.Exception);

                return View(model);
            }
        }
    }
}
