using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.DataAccess.Data;
using ShoppingCart.Models;

namespace ShoppingCart.Utility.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly CartDbContext context;

        public DbInitializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, CartDbContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager; 
            this.context = context;
        }

        public void Initialize()
        {
            try
            {
                if(context.Database.GetPendingMigrations().Count() > 0)
                    context.Database.Migrate();
            }
            catch(Exception)
            {
                throw;
            }
            if(!roleManager.RoleExistsAsync(WebSiteRole.RoleAdmin).GetAwaiter().GetResult())
            {
                roleManager.CreateAsync(new IdentityRole(WebSiteRole.RoleAdmin)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(WebSiteRole.RoleUser)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(WebSiteRole.RoleEmployee)).GetAwaiter().GetResult();
                userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    Name = "Admin",
                    PhoneNumber = "1234567892",
                    Address = "xyz",
                    City = "xyz",
                    State = "xyz",
                    PinCode ="333011"
                }, "Admin@123").GetAwaiter().GetResult();
                ApplicationUser user = context.ApplicationUsers.FirstOrDefault(x => x.Email == "admin@gmail.com");
                userManager.AddToRoleAsync(user, WebSiteRole.RoleAdmin).GetAwaiter().GetResult();
            }
            return;
        }
    }
}
