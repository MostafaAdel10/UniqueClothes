using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Unique.Models;
using Unique.Repository;

namespace Unique.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public HomeController(ICategoryRepository categoryRepository)
        {
            this._categoryRepository = categoryRepository;
        }

        public IActionResult Index()
        {
            //var categories = _categoryRepository.GetAll(includeProperties: "Product");
            var categories = _categoryRepository.GetAll(includeProperties: nameof(Category.Products));

            return View(categories);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
