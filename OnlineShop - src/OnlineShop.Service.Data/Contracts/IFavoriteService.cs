using System;
using System.Collections.Generic;
using System.Text;
using OnlineShop.Models;

namespace OnlineShop.Service.Data.Contracts
{
    public interface IFavoriteService
    {
        bool Add(string productId, string username);

        IEnumerable<ShopUserFavoriteProduct> AllProducts(string username);

        int Delete(string productId, string username);
    }
}
