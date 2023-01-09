using ShoppingCart.DataAccess.Data;
using ShoppingCart.Models;

namespace ShoppingCart.DataAccess.Repository
{
    public sealed class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private CartDbContext context;
        public OrderHeaderRepository(CartDbContext context) : base(context) => this.context = context;

        public void PaymentStatus(int id, string sessionId, string paymentIntendId)
        {
            var orderHeader = context.OrderHeaders.FirstOrDefault(x => x.Id == id);
            orderHeader.DateOfPayment = DateTime.Now;
            orderHeader.PaymentIntendId = paymentIntendId;  
            orderHeader.SessionId = sessionId;
        }

        public void Update(OrderHeader orderHeader)
        {
            context.OrderHeaders.Update(orderHeader);

            //var orderHeaderDb = context.OrderHeaders.FirstOrDefault(x => x.Id == orderHeader.Id);
            //if (orderHeaderDb != null)
            //{
            //    orderHeaderDb.Name = orderHeader.Name;
            //    orderHeaderDb.DisplayOrder = orderHeader.DisplayOrder;
            //}
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var order = context.OrderHeaders.FirstOrDefault(x => x.Id == id);
            if(order != null)
                order.OrderStatus = orderStatus;
            if(paymentStatus != null)
                order.PaymentStatus = paymentStatus;
        }
    }
}
