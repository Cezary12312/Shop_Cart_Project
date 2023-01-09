using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using ShoppingCart.DataAccess.Data;
using ShoppingCart.Models;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.DataAccess.Repository
{
    public sealed class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private CartDbContext context;
        public OrderDetailRepository(CartDbContext context) : base(context) => this.context = context;
        public void Update(OrderDetail orderDetail)
        {
            context.OrderDetails.Update(orderDetail);

            //var categoryDb = context.Categories.FirstOrDefault(x => x.Id == category.Id);
            //if (categoryDb != null)
            //{
            //    categoryDb.Name = category.Name;
            //    categoryDb.DisplayOrder = category.DisplayOrder;
            //}
        }

    }
}
