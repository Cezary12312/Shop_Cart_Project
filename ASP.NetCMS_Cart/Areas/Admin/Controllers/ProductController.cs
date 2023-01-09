using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoppingCart.DataAccess.Repository;
using ShoppingCart.DataAccess.ViewModels;
using System.Data;

namespace ASP.NetCMS_Cart.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private IUnitOfWork unitOfWork;
        private IWebHostEnvironment env;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            this.unitOfWork = unitOfWork;
            this.env = env; 
        }
        public IActionResult Index()
        {
            ProductVM product = new ProductVM();
            product.Products = unitOfWork.ProductRepository.GetAll();
            product.Categories = unitOfWork.CategoryRepository.GetAll().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            return View(product);
        }
        public IActionResult AllProducts()
        {
            var products = unitOfWork.ProductRepository.GetAll(includeProperties:"Category");
            return Json(new { data = products });
        }
        [HttpGet]
        public IActionResult CreateUpdate(int? id)
        {
            ProductVM product = new ProductVM()
            {
                Product = new(),
                Categories = unitOfWork.CategoryRepository.GetAll().Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            if (id == null || id == 0)
                return View(product);
            else
            {
                product.Product = unitOfWork.ProductRepository.GetT(x => x.Id == id);
                if (product.Product == null)
                    return NotFound();
                else
                    return View(product);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUpdate(ProductVM vm, IFormFile? file) //Create too, change in SingleResponsibility
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index");
            string fileName = string.Empty;
            if (file != null)
            {
                string uploadDir = Path.Combine(env.WebRootPath, "ProductImages");
                fileName = Guid.NewGuid().ToString() + "-" + file.FileName;
                string filePath = Path.Combine(uploadDir, fileName);
                vm.Product.ImageUrl = fileName;
                if (vm.Product.ImageUrl != null)
                {
                    var oldImagePath = Path.Combine(env.WebRootPath, vm.Product.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                        file.CopyTo(fileStream);
                    vm.Product.ImageUrl = @"\ProductImage\" + fileName;
                }
            }
            if (vm.Product.Id == 0)
            {
                unitOfWork.ProductRepository.Add(vm.Product);
                TempData["success"] = "Dodano produkt";
            }
            else
            {
                unitOfWork.ProductRepository.Update(vm.Product);
                TempData["success"] = "Zmieniono produkt";
            }
            unitOfWork.Save();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            var product = unitOfWork.ProductRepository.GetT(x => x.Id == id);
            if (product == null)
                return NotFound();
            else
                return View(product);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteData(int? id)
        {
            var product = unitOfWork.ProductRepository.GetT(x => x.Id == id);
            if (product == null)
                return NotFound();
            var oldImagePath = Path.Combine(env.WebRootPath, product.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
                System.IO.File.Delete(oldImagePath);
            unitOfWork.ProductRepository.Remove(product);
            unitOfWork.Save();
            TempData["success"] = "Usunięto produkt";
            return RedirectToAction("Index");
        }
    }
}
