using Microsoft.AspNetCore.Mvc;
using Unique.Models;
using Unique.Repository;

namespace Unique.Controllers
{
    public class ManageCategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ManageCategoryController(ICategoryRepository categoryRepository ,IProductRepository productRepository, IWebHostEnvironment webHostEnvironment)
        {
            this._categoryRepository = categoryRepository;
            this._productRepository = productRepository;
            this._webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var categories = _categoryRepository.GetAll();
            return View(categories);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Image != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(obj.Image.FileName);
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"img\CategoriesImages");

                using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
                obj.Image.CopyTo(fileStream);

                obj.ImageURL = @"\img\CategoriesImages\" + fileName;
            }
            else
            {
                obj.ImageURL = "https://placehold.co/600x400";
            }

            if (obj.Name == obj.Description)
            {
                ModelState.AddModelError("name", "The description cannot exactly match the Name.");
            }
            if (ModelState.IsValid)
            {
                _categoryRepository.Add(obj);
                _categoryRepository.Save();
                TempData["success"] = "The category has been created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        public IActionResult Update(int id)
        {
            var obj = _categoryRepository.GetById(id);
            if (obj == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(obj);
        }

        [HttpPost]
        public IActionResult Update(Category obj)
        {
            if (ModelState.IsValid && obj.CategoryID > 0)
            {
                if (obj.Image != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(obj.Image.FileName);
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"img\CategoriesImages");

                    if (!string.IsNullOrEmpty(obj.ImageURL))
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, obj.ImageURL.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
                    obj.Image.CopyTo(fileStream);

                    obj.ImageURL = @"\img\CategoriesImages\" + fileName;
                }

                _categoryRepository.Update(obj);
                _categoryRepository.Save();
                TempData["success"] = "The category has been updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        public IActionResult Delete(int id)
        {
            Category? obj = _categoryRepository.GetById(id);
            if (obj is null)
            {
                TempData["error"] = "Failed to delete the category.";
            }
            return View(obj);

        }


        [HttpPost]
        public IActionResult Delete(Category obj)
        {
            try
            {
                // الحصول على التصنيف من قاعدة البيانات
                var category = _categoryRepository.GetById(obj.CategoryID);

                if (category == null)
                {
                    TempData["error"] = "The category does not exist.";
                    return RedirectToAction(nameof(Index));
                }

                // التحقق إذا كان التصنيف مرتبطًا بجداول أخرى
                var relatedRecords = _productRepository.GetAll()
                                        .Any(p => p.CategoryID == obj.CategoryID);

                if (relatedRecords)
                {
                    TempData["error"] = "The category cannot be deleted because it is referenced in other records. Please unlink or delete the related records first.";
                    return RedirectToAction(nameof(Index));
                }

                if (!string.IsNullOrEmpty(category.ImageURL))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, category.ImageURL.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // حذف التصنيف
                _categoryRepository.Delete(category.CategoryID);
                _categoryRepository.Save();

                TempData["success"] = "The category has been deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = "An error occurred: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

    }
}
