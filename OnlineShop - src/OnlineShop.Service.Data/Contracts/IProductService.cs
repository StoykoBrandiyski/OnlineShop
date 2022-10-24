using OnlineShop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShop.Service.Data.Contracts
{
    public interface IProductService
    {
        Task<int> AddProduct(Product product);

        Task<Product> HideProduct(string productId);

        Task<bool> EditProduct(Product product);

        Task<Product> ShowProduct(string productId);

        Task<bool> ProductExist(string productId);

        Task<Product> GetProductById(string productId);

        Task<IEnumerable<Product>> GetAllProductByCategoryId(int categoryId);
        
        IEnumerable<Product> GetVisibleProducts();

        IEnumerable<Product> GetProductsBySearch(string searchString);

        IEnumerable<Product> OrderBy(IEnumerable<Product> products, string typeOrder);

        IEnumerable<Product> GetHideProducts();

        IEnumerable<Product> GetProductsByFilter(string type,string value,SubCategory category);

        IEnumerable<Product> GetProductsByFilterPriceOrder(string typeOrder, SubCategory category);

        IEnumerable<Product> GetProductsByFilterPrice(decimal minPrice, decimal maxPrice,SubCategory category);
        
        IEnumerable<Product> GetProductsByFilterBrand(string brand, SubCategory category);

        IEnumerable<Product> GetAllProducts();
        
        int AddImageUrls(string productId, IEnumerable<string> imageUrls);

        IEnumerable<ProductImage> GetImages(string productId);
    }
}
