using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OnlineShop.Models;
using OnlineShop.Models.Enums;
using OnlineShop.Web.Data;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Web.Infrastructure.Middlewares
{
    public class SeedDataMiddleware
    {
        private readonly RequestDelegate _next;

        public SeedDataMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<ShopUser> userManager,
            RoleManager<IdentityRole> roleManager, OnlineShopDbContext dbContext)
        {
            SeedRoles(roleManager).GetAwaiter().GetResult();
            SeedUserInRoles(userManager).GetAwaiter().GetResult();

            await _next(context);
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync(UserRole.Admin.ToString())) 
            {
                await roleManager.CreateAsync(new IdentityRole(UserRole.Admin.ToString()));
            }
            if (!await roleManager.RoleExistsAsync(UserRole.Partner.ToString()))
            {
                await roleManager.CreateAsync(new IdentityRole(UserRole.Partner.ToString()));
            }
        }

        private static async Task SeedUserInRoles(UserManager<ShopUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                ShopUser user = new ShopUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    FirstName = "AdminFirstName",
                    LastName = "AdminLastName",
                    ShoppingCart = new ShoppingCart()
                };

                string password = "1234567";

                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, UserRole.Admin.ToString());
                }
            }
        }

        private static async Task SeedCateParentCategoriesgories()
        {
            
        }

    }
}