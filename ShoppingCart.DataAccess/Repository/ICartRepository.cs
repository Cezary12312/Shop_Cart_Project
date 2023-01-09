using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Repository
{
    public interface ICartRepository : IRepository<Cart>
    {
        void ChangeCartCount(int id, int count);
    }
}
