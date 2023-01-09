using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using ShoppingCart.DataAccess.Repository;
using ShoppingCart.DataAccess.ViewModels;
using ShoppingCart.Models;
using ShoppingCart.Utility;
using System.Data;
using System.Security.Claims;

namespace ASP.NetCMS_Cart.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private IUnitOfWork unitOfWork;
        public OrderController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index(string status = "")
        {
            IEnumerable<OrderHeader> orderHeaders;

            if (User.IsInRole("Admin") || User.IsInRole("Employee"))
                orderHeaders = unitOfWork.OrderHeaderRepository.GetAll(includeProperties: "ApplicationUser");
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                orderHeaders = unitOfWork.OrderHeaderRepository.GetAll(x => x.ApplicationUserId == claims.Value);
            }
            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(x => x.PaymentStatus == PaymentStatus.StatusPending);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(x => x.PaymentStatus == PaymentStatus.StatusApproved);
                    break;
                case "underprocess":
                    orderHeaders = orderHeaders.Where(x => x.OrderStatus == OrderStatus.StatusInProcess);
                    break;
                case "shipped":
                    orderHeaders = orderHeaders.Where(x => x.OrderStatus == OrderStatus.StatusShipped);
                    break;
                default:
                    break;
            }
            return View(orderHeaders.ToList());
        }
        public IActionResult OrderDetails(int id)
        {
            OrderVM orderVM = new OrderVM()
            {
                OrderHeader = unitOfWork.OrderHeaderRepository.GetT(x => x.Id == id,
                    includeProperties: "ApplicationUser"),
                OrderDetails = unitOfWork.OrderDetailRepository.GetAll(x => x.OrderHeaderId == id,
                    includeProperties: "Product")
            };
            return View(orderVM);
        }
        [Authorize(Roles = WebSiteRole.RoleAdmin + "," + WebSiteRole.RoleEmployee)]
        [HttpPost]
        public IActionResult OrderDetails(OrderVM vm)
        {
            var orderHeader = unitOfWork.OrderHeaderRepository.GetT(x => x.Id == vm.OrderHeader.Id);
            orderHeader.Name = vm.OrderHeader.Name;
            orderHeader.Phone = vm.OrderHeader.Phone;
            orderHeader.Address = vm.OrderHeader.Address;
            orderHeader.City = vm.OrderHeader.City;
            orderHeader.State = vm.OrderHeader.State;
            orderHeader.PostalCode = vm.OrderHeader.PostalCode;
            if (vm.OrderHeader.Carrier != null)
                orderHeader.Carrier = vm.OrderHeader.Carrier;     
            if (vm.OrderHeader.TrackingNumber != null)
                orderHeader.TrackingNumber = vm.OrderHeader.TrackingNumber;
            unitOfWork.OrderHeaderRepository.Update(orderHeader);
            unitOfWork.Save();
            TempData["success"] = "Zaaktualizowano";
            return RedirectToAction("OrderDetails", "Order", new {id = vm.OrderHeader.Id });
        }
        [Authorize(Roles = WebSiteRole.RoleAdmin + "," + WebSiteRole.RoleEmployee)]
        public IActionResult InProcess(OrderVM vm)
        {
            unitOfWork.OrderHeaderRepository.UpdateStatus(vm.OrderHeader.Id, OrderStatus.StatusInProcess);
            unitOfWork.Save();
            TempData["success"] = "Zaaktualizowano - w trakcie przygotowania";
            return RedirectToAction("OrderDetails", "Order", new { id = vm.OrderHeader.Id });
        }
        [Authorize(Roles = WebSiteRole.RoleAdmin + "," + WebSiteRole.RoleEmployee)]
        public IActionResult Shipped(OrderVM vm)
        {
            var orderHeader = unitOfWork.OrderHeaderRepository.GetT(x => x.Id == vm.OrderHeader.Id);
            orderHeader.Carrier = vm.OrderHeader.Carrier;
            orderHeader.TrackingNumber = vm.OrderHeader.TrackingNumber;
            orderHeader.OrderStatus = vm.OrderHeader.OrderStatus;
            orderHeader.DateOfShipping = DateTime.Now;
            unitOfWork.OrderHeaderRepository.Update(orderHeader);
            unitOfWork.Save();
            TempData["success"] = "Zaaktualizowano - wysłano";
            return RedirectToAction("OrderDetails", "Order", new { id = vm.OrderHeader.Id });
        }
        [Authorize(Roles = WebSiteRole.RoleAdmin + "," + WebSiteRole.RoleEmployee)]
        public IActionResult CancelOrder(OrderVM vm)
        {
            vm.OrderDetails = unitOfWork.OrderDetailRepository.GetAll(x => x.OrderHeaderId == vm.OrderHeader.Id);
            unitOfWork.OrderDetailRepository.RemoveRange(vm.OrderDetails);
            unitOfWork.Save();
            unitOfWork.OrderHeaderRepository.Remove(vm.OrderHeader);
            unitOfWork.Save();            
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }
        public IActionResult PayNow(OrderVM vm)
        {
            var orderHeader = unitOfWork.OrderHeaderRepository.GetT(x => x.Id == vm.OrderHeader.Id, includeProperties: "ApplicationUser");
            var orderDetails = unitOfWork.OrderDetailRepository.GetAll(x => x.OrderHeaderId == vm.OrderHeader.Id, includeProperties: "Product");
            
            unitOfWork.OrderHeaderRepository.UpdateStatus(orderHeader.Id, OrderStatus.StatusApproved);
            unitOfWork.Save();
            return RedirectToAction("Index", "Home");
        }
    }
}
