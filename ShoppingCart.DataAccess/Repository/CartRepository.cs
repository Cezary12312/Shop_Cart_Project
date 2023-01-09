using ShoppingCart.DataAccess.Data;
using ShoppingCart.Models;

namespace ShoppingCart.DataAccess.Repository
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        private CartDbContext context;
        public CartRepository(CartDbContext context) : base(context) => this.context = context;
        public void ChangeCartCount(int id, int count)
        {
            Cart cart = GetT(x => x.Id ==  id);
            if (cart != null) 
            {
                cart.Count += count;
            }
        }
    }
}
