using OnlineShop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShop.Service.Data.Contracts
{
    public interface IShoppingCartsService
    {
        Task AddProductInShoppingCart(string productId, string username, int? quntity = null);

        Task<int> EditProductQuantityInShoppingCart(string productId, string username, int quantity);

        IEnumerable<ShoppingCartProduct> GetAllShoppingCartProducts(string username);

        Task<int> DeleteProductFromShoppingCart(string producId, string username);

        int DeleteAllProductFromShoppingCart(string username);

        bool AnyProducts(string username);
    }
}
