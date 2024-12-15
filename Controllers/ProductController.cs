using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Unique.Models;
using Unique.Repository;
using Unique.Utility;
using Unique.ViewModels;


namespace Unique.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;

        public ProductController(IProductRepository productRepository,ICartItemRepository cartItemRepository,IShoppingCartRepository shoppingCartRepository)
        {
            this._productRepository = productRepository;
            this._cartItemRepository = cartItemRepository;
            this._shoppingCartRepository = shoppingCartRepository;
        }
        public IActionResult FilterColor(string? color)
        {
            //List<Product> productRepositories = _productRepository.GetAll();
            List<Product> productRepositories = _productRepository.GetProductsColorQueryable(color).ToList();
            return PartialView("_ShopProductsWithFilter", productRepositories);
        }
        public IActionResult FilterPrice(int? to)
        {
            //List<Product> productRepositories = _productRepository.GetAll();
            List<Product> productRepositories = _productRepository.GetProductsQueryable(to).ToList();
            return PartialView("_ShopProductsWithFilter", productRepositories);
        }
        public IActionResult FilterProducts(string? searchText)
        {
            //List<Product> productRepositories = _productRepository.GetAll();
            List<Product> productRepositories = _productRepository.GetProductsQueryable(searchText).ToList();
            return PartialView("_ShopProductsWithFilter", productRepositories);
        }
        public IActionResult GetAllProducts()
        {
            List<Product> productRepositories = _productRepository.GetAll().ToList();
            return View("GetAllProducts", productRepositories);
        }
        [Authorize]
        public IActionResult GetProductById(int id)
        {
            Product productById = _productRepository.GetById(id);
            return View("GetProductById", productById);
        }

        [Authorize]
        public IActionResult Details(int id)
        {
            // استرجاع المنتج
            var product = _productRepository.GetById(id);
            if (product == null)
            {
                TempData["error"] = "Product not found.";
                return RedirectToAction(nameof(GetAllProducts));
            }

            // استرجاع هوية المستخدم
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                TempData["error"] = "User not found.";
                return RedirectToAction(nameof(GetAllProducts));
            }

            // التحقق إذا كان السلة موجودة
            var shoppingCart = _shoppingCartRepository.Get(u => u.UserID == userId);
            if (shoppingCart == null)
            {
                // إذا لم تكن السلة موجودة، يتم إنشاؤها
                shoppingCart = new ShoppingCart { UserID = userId };
                _shoppingCartRepository.Add(shoppingCart);
                _shoppingCartRepository.Save();
            }

            // التحقق إذا كان المنتج موجودًا في العناصر
            var cartItem = _cartItemRepository.Get(u => u.ShoppingCart.UserID == userId && u.ProductID == id);
            if (cartItem != null)
            {
                // إذا كان المنتج موجودًا، يتم تحديث الكمية
                //cartItem.Quantity += product.Stock;
                //_cartItemRepository.Update(cartItem);

                TempData["error"] = "The product is already in the cart.";
            }
            else
            {
                // إذا لم يكن موجودًا، يتم إضافته
                cartItem = new CartItem
                {
                    CartID = shoppingCart.CartID,
                    Price = product.Price,
                    ProductID = product.ProductID,
                    Quantity = 1

                };
                _cartItemRepository.Add(cartItem);
                TempData["success"] = "Cart updated successfully.";
            }

            _cartItemRepository.Save();

            // تحديث الجلسة بعدد العناصر في السلة
            HttpContext.Session.SetInt32(SD.SessionCart,
                _cartItemRepository.GetAll(u => u.ShoppingCart.UserID == userId).Count());
            
            return RedirectToAction(nameof(GetAllProducts));
        }


    }
}
