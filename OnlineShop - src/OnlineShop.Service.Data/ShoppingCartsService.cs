using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;
using OnlineShop.Service.Data.Contracts;
using OnlineShop.Web.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Service.Data
{
    public class ShoppingCartsService : IShoppingCartsService
    {
        private const int DEFAULT_PRODUCT_QUANTITY = 1;

        private readonly OnlineShopDbContext dbContext;
        private readonly IProductService productService;
        private readonly IUserService userService;

        public ShoppingCartsService(OnlineShopDbContext dbContext,
            IProductService productService, IUserService userService)
        {
            this.dbContext = dbContext;
            this.productService = productService;
            this.userService = userService;
        }


        public async Task AddProductInShoppingCart(string productId, string username, int? quantity = null)
        {
            Product product = await this.productService.GetProductById(productId);
            ShopUser user = this.userService.GetUserByUsername(username);

            if (product == null || user == null)
            {
                return;
            }

            ShoppingCartProduct shoppingCart = this.GetShoppingCartProduct(product.Id, user.ShoppingCartId);

            if (shoppingCart != null)
            {
                return;
            }

            shoppingCart = new ShoppingCartProduct
            {
                Product = product,
                Quantity = quantity == null ? DEFAULT_PRODUCT_QUANTITY : quantity.Value,
                ShoppingCartId = user.ShoppingCartId
            };

            await this.dbContext.ShoppingCartProducts.AddAsync(shoppingCart);
            await this.dbContext.SaveChangesAsync();
        }
        
        //N + 1 problem ?
        public bool AnyProducts(string username)
        {
            bool isAnyProducts = this.dbContext.ShoppingCartProducts
                .Any(product => product.ShoppingCart.ShopUser.UserName == username);

            return isAnyProducts;
        }

        public int DeleteAllProductFromShoppingCart(string username)
        {
            ShopUser user = this.userService.GetUserByUsername(username);

            if (user == null)
            {
                return 0;
            }

            var shoppingCartProducts = this.dbContext.ShoppingCartProducts
                        .Where(product => product.ShoppingCartId == user.ShoppingCartId).ToList();

            //Test if list is empty

            this.dbContext.ShoppingCartProducts.RemoveRange(shoppingCartProducts);
            return this.dbContext.SaveChanges();
        }

        public async Task<int> DeleteProductFromShoppingCart(string productId, string username)
        {
            Product productDb = await this.productService.GetProductById(productId);
            ShopUser user = this.userService.GetUserByUsername(username);

            if (productDb == null || user == null) 
            {
                return 0;
            }

            ShoppingCartProduct shoppingCartProduct = this.GetShoppingCartProduct(productDb.Id, user.ShoppingCartId);

            if (shoppingCartProduct == null)
            {
                return 0;
            }

            this.dbContext.ShoppingCartProducts.Remove(shoppingCartProduct);
            return await this.dbContext.SaveChangesAsync();
        }

        public async Task<int> EditProductQuantityInShoppingCart(string productId, string username, int quantity)
        {
            Product product = await this.productService.GetProductById(productId);
            ShopUser user = this.userService.GetUserByUsername(username);

            if (product == null || user == null || quantity <= 0)
            {
                return 0;
            }

            ShoppingCartProduct shoppingCartProduct = this.GetShoppingCartProduct(product.Id, user.ShoppingCartId);

            if (shoppingCartProduct == null)
            {
                return 0;
            }

            shoppingCartProduct.Quantity = quantity;

            this.dbContext.Update(shoppingCartProduct);
            return await this.dbContext.SaveChangesAsync(); 
        }

        //N + 1 problem ?
        public IEnumerable<ShoppingCartProduct> GetAllShoppingCartProducts(string username)
        {
            ShopUser user = this.userService.GetUserByUsername(username);

            if (user == null)
            {
                return new List<ShoppingCartProduct>();
            }

            var shoppingCartProducts = this.dbContext.ShoppingCartProducts
                                    .Include(cartProduct => cartProduct.Product)
                                    .ThenInclude(product => product.Images)
                                    .Include(cardProduct => cardProduct.ShoppingCart)
                                    .Where(cardProduct => cardProduct.ShoppingCart.ShopUser.UserName == username)
                                    .ToList();

            return shoppingCartProducts;
        }

        private ShoppingCartProduct GetShoppingCartProduct(string productId, string shoppingCartId)
        {
            ShoppingCartProduct cartProductDb = this.dbContext.ShoppingCartProducts
                                        .FirstOrDefault(cardProduct => cardProduct.ShoppingCartId == shoppingCartId
                                                        && cardProduct.ProductId == productId);

            return cartProductDb;
        }
    }
}
