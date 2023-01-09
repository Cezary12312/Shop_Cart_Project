using Microsoft.AspNetCore.Mvc;
using ShoppingCart.DataAccess.Repository;
using System.Security.Claims;

namespace ASP.NetCMS_Cart.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork unitOfWork;
        public CartViewComponent(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claims != null)
            {
                if (HttpContext.Session.GetInt32("SessionCart") != null)
                    return View(HttpContext.Session.GetInt32("SessionCart"));
                else
                {
                    HttpContext.Session.SetInt32("SessionCart", unitOfWork.CartRepository.GetAll(x =>
                    x.ApplicationUserId == claims.Value).ToList().Count);
                    return View(HttpContext.Session.GetInt32("SessionCart"));
                }
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
