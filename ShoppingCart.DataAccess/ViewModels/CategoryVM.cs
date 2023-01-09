using ShoppingCart.Models;

namespace ShoppingCart.DataAccess.ViewModels
{
    public sealed class CategoryVM
    {
        public Category Category { get; set; } = new Category();
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    }
}
