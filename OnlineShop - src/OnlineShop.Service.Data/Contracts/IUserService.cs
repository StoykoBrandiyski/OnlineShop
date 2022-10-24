using OnlineShop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShop.Service.Data.Contracts
{
    public interface IUserService
    {
        ShopUser GetUserByUsername(string username);

        Task<IEnumerable<ShopUser>> GetUsersByRole(string role);

        Task<int> CreateCompany(Company company, string username);

        bool AddUserToRole(string username, string role);

        bool RemoveUserFromRole(string username, string role);

        Company GetUserCompanyByUsername(string username);

        bool EditFirstName(ShopUser user, string firstName);

        bool EditLastName(ShopUser user, string lastName);

        ShopUser GetUserByUsernameWithFavoriteProduct(string username);
    }
}
