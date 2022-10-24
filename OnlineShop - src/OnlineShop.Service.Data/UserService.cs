using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;
using OnlineShop.Service.Data.Contracts;
using OnlineShop.Web.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Service.Data
{
    public class UserService : IUserService
    {
        private readonly UserManager<ShopUser> userManager;
        private readonly OnlineShopDbContext dbContext;

        public UserService(UserManager<ShopUser> userManager, OnlineShopDbContext dbContext)
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
        }

        public bool AddUserToRole(string username, string role)
        {
            ShopUser user = GetUserByUsername(username);

            if (user == null)
            {
                return false;
            }

            this.userManager.AddToRoleAsync(user, role);
            
            return true;
        }

        public async Task<int> CreateCompany(Company company, string username)
        {
            if(company == null)
            {
                return 0;
            }

            ShopUser user = GetUserByUsername(username);

            if (user == null)
            {
                return 0;
            }

            user.Company = company;
            return await this.dbContext.SaveChangesAsync();    
        }

        public bool EditFirstName(ShopUser user, string firstName)
        {
            if(user == null || string.IsNullOrEmpty(firstName))
            {
                return false;
            }

            user.FirstName = firstName;

            this.dbContext.SaveChanges();
            return true;
        }

        public bool EditLastName(ShopUser user, string lastName)
        {
            if (user == null || string.IsNullOrEmpty(lastName))
            {
                return false;
            }

            user.LastName = lastName;

            this.dbContext.SaveChanges();
            return true;
        }

        public ShopUser GetUserByUsername(string username)
        {
            ShopUser user = this.userManager.FindByNameAsync(username)
                                .GetAwaiter()
                                .GetResult();

            return user;
        }

        public ShopUser GetUserByUsernameWithFavoriteProduct(string username)
        {
            ShopUser userDb = this.dbContext.Users.Include(x => x.FavoriteProducts)
                .FirstOrDefault(x => x.UserName == username);

            return userDb;
        }

        public Company GetUserCompanyByUsername(string username)
        {
            Company userCompany = this.dbContext.Companies
                          .Include(company => company.Address.City)
                          .FirstOrDefault(company => company.ShopUser.UserName == username);

            return userCompany;
        }

        public async Task<IEnumerable<ShopUser>> GetUsersByRole(string role)
        {
            var usersOfRole = this.userManager.GetUsersInRoleAsync(role).GetAwaiter().GetResult();

            IEnumerable<ShopUser> users = await this.dbContext.Users.Include(x => x.Company)
                                             .ThenInclude(x => x.Address)
                                             .ThenInclude(x => x.City)
                                             .Where(x => usersOfRole.Any(u => u.Id == x.Id))
                                             .ToListAsync();
            return users;
        }

        public bool RemoveUserFromRole(string username, string role)
        {
            ShopUser user = GetUserByUsername(username);

            if (user == null)
            {
                return false;
            }

            this.userManager.RemoveFromRoleAsync(user, role).GetAwaiter().GetResult();

            return true;
        }       
    }
}
