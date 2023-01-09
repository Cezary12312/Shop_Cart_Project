using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.DataAccess.Repository;
using ShoppingCart.DataAccess.ViewModels;

namespace ASP.NetCMS_Cart.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class CategoryController : Controller
    {
        private IUnitOfWork unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }   
        public IActionResult Index()
        {
            CategoryVM category = new CategoryVM();
            category.Categories = unitOfWork.CategoryRepository.GetAll();
            return View(category);
        }
        [HttpGet]
        public IActionResult CreateUpdate(int? id)
        {
            CategoryVM category = new CategoryVM();
            if (id == null || id == 0)
                return View(category);
            else
            {
                category.Category = unitOfWork.CategoryRepository.GetT(x => x.Id == id);
                if (category.Category == null)
                    return NotFound();
                else
                    return View(category);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUpdate(CategoryVM vm) //Create and Update together, change in SingleResponsibility
        {
            if (ModelState.IsValid)
            {
                if (vm.Category.Id == 0)
                {
                    unitOfWork.CategoryRepository.Add(vm.Category);
                    TempData["success"] = "Dodano kategorię";
                }
                else
                {
                    unitOfWork.CategoryRepository.Update(vm.Category);
                    TempData["success"] = "Zaktualizowano kategorię";
                }
                unitOfWork.Save();
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(int? id) 
        {
            if (id == null || id == 0)
                return NotFound();
            var category = unitOfWork.CategoryRepository.GetT(x => x.Id == id);
            if (category == null)
                return NotFound();
            else
                return View(category);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteData(int? id)
        {
            var category = unitOfWork.CategoryRepository.GetT(x => x.Id == id);
            if (category == null)
                return NotFound();
            unitOfWork.CategoryRepository.Remove(category);
            unitOfWork.Save();
            TempData["success"] = "Usunięto kategorię";
            return RedirectToAction("Index");
        }
    }
}
