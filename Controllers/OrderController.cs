using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;
using Unique.Models;
using Unique.Repository;
using Unique.Utility;
using Unique.ViewModels;

namespace Unique.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;

        [BindProperty]
        public OrderVM OrderVM { get; set; }
        public OrderController(IOrderRepository orderRepository,IOrderDetailRepository orderDetailRepository)
        {
            this._orderRepository = orderRepository;
            this._orderDetailRepository = orderDetailRepository;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int orderId)
        {
            OrderVM = new()
            {
                OrderHeader = _orderRepository.Get(u => u.OrderID == orderId, includeProperties:"User"),
                OrderDetail = _orderDetailRepository.GetAll(u => u.OrderID == orderId, includeProperties:"Product")
            };

            return View(OrderVM);
        }


        [HttpGet]
        public IActionResult GetAllOrders(string status)
        {
            IEnumerable<Order> objOrderHeaders;


            if (User.IsInRole(SD.Role_Admin))
            {
                objOrderHeaders = _orderRepository.GetAll(includeProperties: "User").ToList();
            }
            else
            {

                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                objOrderHeaders = _orderRepository.GetAll(u => u.UserID == userId, includeProperties: "User").ToList();
            }
            switch (status)
            {
                case "pending":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusPending).ToList();
                    break;
                case "Shipped":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusShipped).ToList();
                    break;
                case "completed":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusApproved).ToList();
                    break;
                default:
                    break;

            }
            return View(objOrderHeaders);
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult UpdateOrderDetail()
        {
            var orderHeaderFromDb = _orderRepository.Get(u => u.OrderID == OrderVM.OrderHeader.OrderID);
            orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.State = OrderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
            {
                orderHeaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }
            _orderRepository.Update(orderHeaderFromDb);
            _orderRepository.Save();

            TempData["Success"] = "Order Details Updated Successfully.";


            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.OrderID });
        }



        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult ShipOrder()
        {
            var orderHeader = _orderRepository.Get(u => u.OrderID == OrderVM.OrderHeader.OrderID);
            orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;


            _orderRepository.Update(orderHeader);
            _orderRepository.Save();
            TempData["Success"] = "Order Shipped Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.OrderID });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult DoneOrder()
        {
            var orderHeader = _orderRepository.Get(u => u.OrderID == OrderVM.OrderHeader.OrderID);

            orderHeader.OrderStatus = SD.StatusApproved;


            _orderRepository.Update(orderHeader);
            _orderRepository.Save();
            TempData["Success"] = "Order Done Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.OrderID });
        }


        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult CancelOrder()
        {
            var orderHeader = _orderRepository.Get(u => u.OrderID == OrderVM.OrderHeader.OrderID);

            _orderRepository.UpdateStatus(orderHeader.OrderID, SD.StatusCancelled);
            _orderRepository.Save();

      
            TempData["Success"] = "Order Cancelled Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.OrderID });
        }





        #region API CALLS

        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<Order> objOrderHeaders;


            if (User.IsInRole(SD.Role_Admin))
            {
                objOrderHeaders = _orderRepository.GetAll(includeProperties:"User").ToList();
            }
            else
            {

                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                objOrderHeaders = _orderRepository.GetAll(u => u.UserID == userId, includeProperties:"User");
            }


            switch (status)
            {
                case "pending":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusPending);
                    break;
                case "inprocess":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;

            }


            return Json(new { data = objOrderHeaders });
        }


        #endregion
    }
}
