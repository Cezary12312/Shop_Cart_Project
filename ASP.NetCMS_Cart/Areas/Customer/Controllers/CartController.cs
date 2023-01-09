using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.DataAccess.Repository;
using ShoppingCart.DataAccess.ViewModels;
using ShoppingCart.Models;
using ShoppingCart.Utility;
using System.Security.Claims;

namespace ASP.NetCMS_Cart.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private IUnitOfWork unitOfWork;
        public CartVM vm { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            vm = new CartVM()
            {
                ListOfCart = unitOfWork.CartRepository.GetAll(x => x.ApplicationUserId == claims.Value, includeProperties: "Product"),
                ItemOrderHeader = new OrderHeader()
            };

            foreach (var item in vm.ListOfCart)
                vm.ItemOrderHeader.OrderTotal += (item.Product.Price * item.Count);
            return View(vm);
        }

        [HttpPost]
        public IActionResult Summary(CartVM vm)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            vm.ListOfCart = unitOfWork.CartRepository.GetAll(
                x => x.ApplicationUserId == claims.Value, 
                includeProperties:"Product");
            vm.ItemOrderHeader.OrderStatus = OrderStatus.StatusPending;
            vm.ItemOrderHeader.PaymentStatus = PaymentStatus.StatusPending;
            vm.ItemOrderHeader.DateOfOrder= DateTime.Now;
            vm.ItemOrderHeader.ApplicationUserId = claims.Value;

            foreach (var item in vm.ListOfCart)
                vm.ItemOrderHeader.OrderTotal += (item.Product.Price * item.Count);
            unitOfWork.OrderHeaderRepository.Add(vm.ItemOrderHeader);
            unitOfWork.Save();

            foreach (var item in vm.ListOfCart)
            {
                OrderDetail detail = new OrderDetail()
                {
                    OrderHeaderId = vm.ItemOrderHeader.Id,
                    ProductId = item.ProductId,
                    Price = item.Product.Price,
                    Count = item.Count
                };
                unitOfWork.OrderDetailRepository.Add(detail);
                unitOfWork.Save();
            }

            unitOfWork.CartRepository.RemoveRange(vm.ListOfCart);
            unitOfWork.Save();

            return RedirectToAction("Index", "Home");
        }
        public IActionResult OrderSuccess(int id)
        {
            OrderHeader orderHeader = unitOfWork.OrderHeaderRepository.GetT(x => x.Id == id);
            List<Cart> carts = unitOfWork.CartRepository.GetAll(x => x.ApplicationUserId == orderHeader.ToString()).ToList();
            unitOfWork.CartRepository.RemoveRange(carts);
            unitOfWork.Save();
            return View(id);
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            vm = new CartVM()
            {
                ListOfCart = unitOfWork.CartRepository.GetAll(x => x.ApplicationUserId == claims.Value, includeProperties: "Product"),
                ItemOrderHeader = new OrderHeader()
            };
            vm.ItemOrderHeader.ApplicationUser = unitOfWork.ApplicationUserRepository.GetT(
                x => x.Id == claims.Value);
            vm.ItemOrderHeader.Name = vm.ItemOrderHeader.ApplicationUser.Name;
            vm.ItemOrderHeader.Phone = vm.ItemOrderHeader.ApplicationUser.Phone;
            vm.ItemOrderHeader.Address = vm.ItemOrderHeader.ApplicationUser.Address;
            vm.ItemOrderHeader.City = vm.ItemOrderHeader.ApplicationUser.City;
            vm.ItemOrderHeader.State = vm.ItemOrderHeader.ApplicationUser.State;
            vm.ItemOrderHeader.PostalCode = vm.ItemOrderHeader.ApplicationUser.PinCode;

            foreach (var item in vm.ListOfCart)
                vm.ItemOrderHeader.OrderTotal += (item.Product.Price * item.Count);

            return View(vm);
        }
        public async Task<IActionResult> Plus(int id)
        {
            var cart = unitOfWork.CartRepository.GetT(x => x.ProductId == id);
            unitOfWork.CartRepository.ChangeCartCount(cart.Id, 1);
            unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Minus(int id)
        {
            var cart = unitOfWork.CartRepository.GetT(x => x.ProductId == id);
            if (cart.Count <= 1)
            {
                unitOfWork.CartRepository.Remove(cart);
                var count = unitOfWork.CartRepository.GetAll(x => x.ApplicationUserId == cart.ApplicationUserId).ToList().Count - 1;
                HttpContext.Session.SetInt32("SessionCart", count);
            }
            else
                unitOfWork.CartRepository.ChangeCartCount(cart.Id, -1);
            unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            var cart = unitOfWork.CartRepository.GetT(x => x.ProductId == id);
            unitOfWork.CartRepository.Remove(cart);
            unitOfWork.Save();
            var count = unitOfWork.CartRepository.GetAll(x => x.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32("SessionCart", count);
            return RedirectToAction(nameof(Index));
        }
    }
}
