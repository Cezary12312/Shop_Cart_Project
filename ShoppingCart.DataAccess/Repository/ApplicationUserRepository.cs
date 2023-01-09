using ShoppingCart.DataAccess.Data;
using ShoppingCart.Models;

namespace ShoppingCart.DataAccess.Repository
{
    public sealed class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private CartDbContext context;
        public ApplicationUserRepository(CartDbContext context) : base(context) => this.context = context;
        public void Update(ApplicationUser applicationUser)
        {
            var applicationUserDb = context.ApplicationUsers.FirstOrDefault(x => x.Name == applicationUser.Name);
            if (applicationUserDb != null)
            {
                applicationUserDb.Phone = applicationUser.Phone;
            }
        }
    }
}
