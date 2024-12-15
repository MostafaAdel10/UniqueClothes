using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Unique.Repository;
using Unique.ViewModels;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Unique.Utility;


namespace Unique.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IEmailSender _emailSender;
        private readonly IApplicationUserRepository _applicationUserRepository;

        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public ShoppingCartController(ICartItemRepository cartItemRepository, IEmailSender emailSender,IApplicationUserRepository applicationUserRepository)
        {
            this._cartItemRepository = cartItemRepository;
            this._emailSender = emailSender;
            this._applicationUserRepository = applicationUserRepository;
        }


        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _cartItemRepository.GetAll(u => u.ShoppingCart.UserID == userId,
                 includeProperties: "Product"),
                OrderHeader = new()
            };

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                ShoppingCartVM.OrderHeader.OrderTotal += ((double)(cart.Product.Price * cart.Quantity));
            }

            return View(ShoppingCartVM);
        }

     
        public IActionResult Plus(int cartId)
        {
            //var cartFromDb = _cartItemRepository.Get(u => u.CartItemID == cartId);

            // جلب العنصر من قاعدة البيانات مع تضمين المنتج
            var cartFromDb = _cartItemRepository.Get(
                u => u.CartItemID == cartId,
                includeProperties: "Product"
            );

            if (cartFromDb.Quantity < cartFromDb.Product.Stock)
            {
                cartFromDb.Quantity += 1;
                _cartItemRepository.Update(cartFromDb);
                _cartItemRepository.Save();
                TempData["success"] = "Item quantity updated successfully.";
            }
            else
            {
                TempData["error"] = $"Insufficient stock. Available stock: {cartFromDb.Product.Stock} item.";
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            // استرجاع العنصر من قاعدة البيانات
            var cartFromDb = _cartItemRepository.Get(u => u.CartItemID == cartId);

            // التحقق من وجود العنصر
            if (cartFromDb == null)
            {
                TempData["error"] = "Cart item not found.";
                return RedirectToAction(nameof(Index));
            }

            // إذا كانت الكمية 1 أو أقل، قم بحذف العنصر
            if (cartFromDb.Quantity <= 1)
            {
                _cartItemRepository.Delete(cartFromDb.CartItemID);

                // تحديث الجلسة بعد حذف العنصر
                var updatedCartCount = _cartItemRepository
                    .GetAll(u => u.CartID == cartFromDb.CartID)
                    .Count();
                HttpContext.Session.SetInt32(SD.SessionCart, updatedCartCount);
            }
            else
            {
                // تقليل الكمية بمقدار 1
                cartFromDb.Quantity -= 1;
                _cartItemRepository.Update(cartFromDb);
            }

            // حفظ التغييرات في قاعدة البيانات
            _cartItemRepository.Save();

            // رسالة نجاح
            TempData["success"] = "Item quantity updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            // استرجاع العنصر من قاعدة البيانات
            var cartItem = _cartItemRepository.Get(u => u.CartItemID == cartId);
            if (cartItem == null)
            {
                TempData["error"] = "Cart item not found.";
                return RedirectToAction(nameof(Index));
            }

            // حذف العنصر
            _cartItemRepository.Delete(cartItem.CartItemID);

            // تحديث الجلسة
            var userId = cartItem.CartID;
            var cartCount = _cartItemRepository.GetAll(u => u.CartID == userId).Count();
            HttpContext.Session.SetInt32(SD.SessionCart, cartCount);

            // حفظ التغييرات
            _cartItemRepository.Save();

            TempData["success"] = "Item removed successfully.";
            return RedirectToAction(nameof(Index));
        }


    }
}
