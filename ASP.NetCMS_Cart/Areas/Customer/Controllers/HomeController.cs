using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.DataAccess.Repository;
using ShoppingCart.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace ASP.NetCMS_Cart.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IUnitOfWork unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = unitOfWork.ProductRepository.GetAll(includeProperties: "Category");
            return View(products);
        }
        [HttpGet]
        public IActionResult Details(int? productId)
        {
            Cart cart = new Cart()
            {
                Product = unitOfWork.ProductRepository.GetT(x => x.Id == productId, includeProperties: "Category"),
                Count = 1,
                ProductId = (int)productId
            };
            return View(cart);
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(Cart cart)
        {
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cart.ApplicationUserId = claims.Value;
                cart.AppUser = claims.Subject.Name;

                var cartItem = unitOfWork.CartRepository.GetT(
                    x => x.ProductId == cart.ProductId && x.ApplicationUserId == claims.Value);
                if (cartItem == null)
                {
                    unitOfWork.CartRepository.Add(cart);
                    unitOfWork.Save();
                    HttpContext.Session.SetInt32("Session Cart", unitOfWork.CartRepository
                        .GetAll(x => x.AppUser == claims.Value).ToList().Count);
                }
                else
                {
                    unitOfWork.CartRepository.ChangeCartCount(cartItem.Id, cart.Count);
                    unitOfWork.Save();
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}