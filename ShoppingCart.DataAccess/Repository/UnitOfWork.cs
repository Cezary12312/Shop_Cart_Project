using ShoppingCart.DataAccess.Data;

namespace ShoppingCart.DataAccess.Repository
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private CartDbContext context;
        public ICategoryRepository CategoryRepository { get; private set; }
        public IProductRepository ProductRepository { get; private set; }

        public ICartRepository CartRepository { get; private set; }
        public IApplicationUserRepository ApplicationUserRepository { get; private set; }
        public IOrderHeaderRepository OrderHeaderRepository { get; private set; }
        public IOrderDetailRepository OrderDetailRepository { get; private set; }

        public UnitOfWork(CartDbContext context)
        {
            this.context = context;
            CategoryRepository = new CategoryRepository(context);
            ProductRepository = new ProductRepository(context);
            CartRepository = new CartRepository(context);
            ApplicationUserRepository = new ApplicationUserRepository(context);
            OrderHeaderRepository = new OrderHeaderRepository(context);
            OrderDetailRepository = new OrderDetailRepository(context);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
