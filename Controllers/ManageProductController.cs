using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Unique.Models;
using Unique.Repository;

namespace Unique.Controllers
{
    public class ManageProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ManageProductController(IProductRepository productRepository, ICategoryRepository categoryRepository, IWebHostEnvironment webHostEnvironment)
        {
            this._productRepository = productRepository;
            this._categoryRepository = categoryRepository;
            this._webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var products = _productRepository.GetAll();
            return View(products);
        }
        public IActionResult Create()
        {
            ViewData["CategoryList"] = _categoryRepository.GetAll();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product obj)
        {
            if (obj.Image != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(obj.Image.FileName);
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"img\ProductsImages");

                using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
                obj.Image.CopyTo(fileStream);

                obj.ImageURL1 = @"\img\ProductsImages\" + fileName;
            }
            else
            {
                obj.ImageURL1 = "https://placehold.co/600x400";
            }
            

            if (obj.Name == obj.Description)
            {
                ModelState.AddModelError("name", "The description cannot exactly match the Name.");
            }
            if (ModelState.IsValid)
            {
                _productRepository.Add(obj);
                _productRepository.Save();
                TempData["success"] = "The product has been created successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryList"] = _categoryRepository.GetAll();
            return View();
        }
        public IActionResult Update(int id)
        {
            ViewData["CategoryList"] = _categoryRepository.GetAll();

            var obj = _productRepository.GetById(id);
            if (obj == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(obj);
        }

        [HttpPost]
        public IActionResult Update(Product obj)
        {
            if (ModelState.IsValid && obj.ProductID > 0)
            {
                if (obj.Image != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(obj.Image.FileName);
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"img\ProductsImages");

                    if (!string.IsNullOrEmpty(obj.ImageURL1))
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, obj.ImageURL1.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
                    obj.Image.CopyTo(fileStream);

                    obj.ImageURL1 = @"\img\ProductsImages\" + fileName;
                }
                
                _productRepository.Update(obj);
                _productRepository.Save();
                TempData["success"] = "The product has been updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryList"] = _categoryRepository.GetAll();
            return View(obj);
        }


        public IActionResult Delete(int id)
        {
            Product? obj = _productRepository.GetById(id);
            if (obj is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(obj);
        }


        [HttpPost]
        public IActionResult Delete(Product obj)
        {

            var deleted = _productRepository.GetById(obj.ProductID);
            if (deleted != null)
            {
                if (!string.IsNullOrEmpty(deleted.ImageURL1))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, deleted.ImageURL1.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                if (!string.IsNullOrEmpty(deleted.ImageURL2))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, deleted.ImageURL2.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                if (!string.IsNullOrEmpty(deleted.ImageURL3))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, deleted.ImageURL3.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                _productRepository.Delete(deleted.ProductID);
                _productRepository.Save();
                TempData["success"] = "The product has been deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = "Failed to delete the product.";
            }
            return View();
        }
    }
}
