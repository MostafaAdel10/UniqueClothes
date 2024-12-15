using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Unique.Repository;
using Unique.ViewModels;
using Microsoft.AspNetCore.Identity.UI.Services;
using Unique.Models;
using Unique.Utility;
using Stripe.Checkout;

namespace Unique.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IEmailSender _emailSender;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;

        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CheckoutController(ICartItemRepository cartItemRepository, IEmailSender emailSender,
            IApplicationUserRepository applicationUserRepository,IOrderRepository orderRepository,IOrderDetailRepository orderDetailRepository)
        {
            this._cartItemRepository = cartItemRepository;
            this._emailSender = emailSender;
            this._applicationUserRepository = applicationUserRepository;
            this._orderRepository = orderRepository;
            this._orderDetailRepository = orderDetailRepository;
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
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _cartItemRepository.GetAll(u => u.ShoppingCart.UserID == userId,
                includeProperties: "Product"),
                OrderHeader = new()
            };

            ShoppingCartVM.OrderHeader.UserID = _applicationUserRepository.Get(u => u.Id == userId).Id;
            
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                //cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += ((double)(cart.Product.Price * cart.Quantity));
            }
            return View(ShoppingCartVM);
        }



        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPOST()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM.ShoppingCartList = _cartItemRepository.GetAll(u => u.ShoppingCart.UserID == userId,
                includeProperties: "Product");

            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.UserID = userId;

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.PostalCode;


            ApplicationUser applicationUser = _applicationUserRepository.Get(u => u.Id == userId);


            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                //cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += ((double)(cart.Product.Price * cart.Quantity));
            }

            //it is a regular customer 
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            
            _orderRepository.Add(ShoppingCartVM.OrderHeader);
            _orderRepository.Save();
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductID = cart.ProductID,
                    OrderID = ShoppingCartVM.OrderHeader.OrderID,
                    Price = cart.Price,
                    Quantity = cart.Quantity
                };
                _orderDetailRepository.Add(orderDetail);
                _orderDetailRepository.Save();
            }

            if (applicationUser.Email != null)
            {
                //it is a regular customer account and we need to capture payment
                //stripe logic
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";
                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain + $"customer/Checkout/OrderConfirmation?id={ShoppingCartVM.OrderHeader.OrderID}",
                    CancelUrl = domain + $"customer/Checkout/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach (var item in ShoppingCartVM.ShoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name
                            }
                        },
                        Quantity = item.Quantity
                    };
                    options.LineItems.Add(sessionLineItem);
                }


                var service = new SessionService();
                Session session = service.Create(options);
                _orderRepository.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.OrderID, session.Id, session.PaymentIntentId);
                _orderRepository.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);

            }

            return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.OrderID });
        }

        public IActionResult OrderConfirmation(int id)
        {
            Order orderHeader = _orderRepository.Get(u => u.OrderID == id, includeProperties: "ApplicationUser");
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                //this is an order by customer

                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _orderRepository.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
                    _orderRepository.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    _orderRepository.Save();
                }
                HttpContext.Session.Clear();
            }

            _emailSender.SendEmailAsync(orderHeader.User.Email, "New Order - Unique Clothes",
                $"<p>New Order Created - {orderHeader.OrderID}</p>");

            List<CartItem> shoppingCarts = _cartItemRepository
                .GetAll(u => u.ShoppingCart.UserID == orderHeader.UserID).ToList();

            _cartItemRepository.RemoveRange(shoppingCarts);
            _cartItemRepository.Save();

            return View(id);
        }







        [HttpPost]
        public IActionResult CashOnDelivery()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM.ShoppingCartList = _cartItemRepository.GetAll(u => u.ShoppingCart.UserID == userId,
                includeProperties: "Product");

            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.UserID = userId;

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.PostalCode;

            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ShippingDate = DateTime.Now.AddDays(7);



            ApplicationUser applicationUser = _applicationUserRepository.Get(u => u.Id == userId);


            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                //cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += ((double)(cart.Product.Price * cart.Quantity));
            }
            ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;

            _orderRepository.Add(ShoppingCartVM.OrderHeader);
            _orderRepository.Save();
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductID = cart.ProductID,
                    OrderID = ShoppingCartVM.OrderHeader.OrderID,
                    Price = cart.Product.Price,
                    Quantity = cart.Quantity
                };
                cart.Product.Stock -= cart.Quantity;

                _orderDetailRepository.Add(orderDetail);
                _orderDetailRepository.Save();  
            }

            return RedirectToAction(nameof(OrderConfirmationCashOnDelivery), new { id = ShoppingCartVM.OrderHeader.OrderID });
        }

        public IActionResult OrderConfirmationCashOnDelivery(int id)
        {
            Order orderHeader = _orderRepository.Get(u => u.OrderID == id, includeProperties: "User");
            

            _emailSender.SendEmailAsync(orderHeader.User.Email, "New Order - Unique Clothes",
                $"<p>New Order Created - {orderHeader.OrderID}</p>");

            List<CartItem> shoppingCarts = _cartItemRepository
                .GetAll(u => u.ShoppingCart.UserID == orderHeader.UserID).ToList();

            _cartItemRepository.RemoveRange(shoppingCarts);
            _cartItemRepository.Save();

            return View(id);
        }
    }
}
