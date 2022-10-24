using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;
using OnlineShop.Service.Data.Contracts;
using OnlineShop.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineShop.Service.Data
{
    public class FavoriteService : IFavoriteService
    {
        private readonly OnlineShopDbContext dbContext;
        private readonly IUserService userService;

        public FavoriteService(OnlineShopDbContext dbContext,IUserService userService)
        {
            this.dbContext = dbContext;
            this.userService = userService;
        }

        public bool Add(string productId, string username)
        {

            //ShopUser user = this.dbContext.Users.Include(x => x.FavoriteProducts)
               // .FirstOrDefault(x => x.UserName == username);
            ShopUser user = this.userService.GetUserByUsernameWithFavoriteProduct(username); //FavoriteProducts is null
            if (user == null)
            {
                return false;
            }

            bool isFavoriteProductExist = user.FavoriteProducts
                                    .Any(product => product.ProductId == productId);
            
            bool isProductExist = this.dbContext.Products.Any(product => product.Id == productId);

            if (isFavoriteProductExist || !isProductExist)
            {
                return false;
            }

            ShopUserFavoriteProduct userFavoriteProduct = new ShopUserFavoriteProduct
            {
                ProductId = productId,
                ShopUserId = user.Id
            };

            user.FavoriteProducts.Add(userFavoriteProduct);
            this.dbContext.SaveChanges();

            return true;
        }

        //N + 1 problem ?
        public IEnumerable<ShopUserFavoriteProduct> AllProducts(string username)
        {
            var favoriteProducts = this.dbContext.ShopUserFavoriteProducts
                                        .Include(fp => fp.Product)
                                        .ThenInclude(product => product.Images)
                                        .Where(fp => fp.ShopUser.UserName == username);

            if (favoriteProducts == null)
            {
                return new List<ShopUserFavoriteProduct>();
            }

            return favoriteProducts;
        }

        //N + 1 problem ?
        public int Delete(string productId, string username)
        {
            ShopUserFavoriteProduct userFavoriteProduct = this.dbContext.ShopUserFavoriteProducts
                            .FirstOrDefault(fp => fp.ShopUser.UserName == username 
                                                && fp.ProductId == productId);

            if (userFavoriteProduct == null)
            {
                return 0;
            }

            this.dbContext.ShopUserFavoriteProducts.Remove(userFavoriteProduct);
            return this.dbContext.SaveChanges();
        }
    }
}
