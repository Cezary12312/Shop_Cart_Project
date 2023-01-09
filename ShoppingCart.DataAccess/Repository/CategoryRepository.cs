using ShoppingCart.DataAccess.Data;
using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Repository
{
    public sealed class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private CartDbContext context;
        public CategoryRepository(CartDbContext context) : base(context) => this.context = context;
        public void Update(Category category)
        {
            var categoryDb = context.Categories.FirstOrDefault(x => x.Id == category.Id);
            if (categoryDb != null)
            {
                categoryDb.Name = category.Name;
                categoryDb.DisplayOrder = category.DisplayOrder;
            }
        }
    }
}
