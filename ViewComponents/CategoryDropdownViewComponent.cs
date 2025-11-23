using Microsoft.AspNetCore.Mvc;
using PetShop.Services.Interfaces;

namespace PetShop.ViewComponents
{
    public class CategoryDropdownViewComponent : ViewComponent
    {
        private readonly ICategoryService _categoryService;

        public CategoryDropdownViewComponent(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = _categoryService.GetAllCategories();
            return View(categories);
        }
    }
}
