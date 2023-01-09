using ShoppingCart.DataAccess.Data;
using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Repository
{
    public sealed class ProductRepository : Repository<Product>, IProductRepository
    {
        private CartDbContext context;
        public ProductRepository(CartDbContext context) : base(context)
        {
            this.context = context;
        }
        public void Update(Product product)
        {
            var productDb = context.Products.FirstOrDefault(x => x.Id == product.Id);
            if (productDb != null)
            {
                productDb.Name = product.Name;
                productDb.Description = product.Description;
                productDb.Price = product.Price;
                if (product.ImageUrl != null)
                    productDb.ImageUrl = product.ImageUrl;
            }
        }
    }
}
