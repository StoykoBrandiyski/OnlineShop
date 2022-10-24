using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;
using OnlineShop.Service.Data.Contracts;
using OnlineShop.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Service.Data
{
    public class ProductService : IProductService
    {
        private readonly OnlineShopDbContext dbContext;

        public ProductService(OnlineShopDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        public async Task<int> AddProduct(Product product)
        {
            if(product == null)
            {
                throw new ArgumentNullException("product");
            }

            await this.dbContext.Products.AddAsync(product);
            return await this.dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> GetAllProductByCategoryId(int categoryId)
        {
            List<Product> products = await this.dbContext.Products
                            .Where(product => product.SubCategoryId == categoryId && product.IsHide == false)
                            .Include(x => x.Images)
                            .Include(product => product.SubCategory)
                            .ToListAsync();

            return products;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            var products = this.dbContext.Products
                                    .Include(product => product.SubCategory)
                                    .ThenInclude(categoty => categoty.ParentCategory)
                                    .Include(product => product.Images)
                                    .ToList();
            return products;
        }

        public IEnumerable<Product> GetHideProducts()
        {
            var products = this.dbContext.Products.Where(product => product.IsHide == true)
                                   .Include(product => product.SubCategory)
                                   .ThenInclude(categoty => categoty.ParentCategory)
                                   .Include(product => product.Images)
                                   .ToList();
            return products;
        }

        public async Task<Product> GetProductById(string productId)
        {
            Product productDb = await this.dbContext.Products
                                        .Include(product => product.Images)
                                        .FirstOrDefaultAsync(product => product.Id == productId && product.IsHide == false);

            return productDb;                                 
        }

        public IEnumerable<Product> GetVisibleProducts()
        {
            var products = this.dbContext.Products.Where(product => product.IsHide == false)
                                  .Include(product => product.SubCategory)
                                  .ThenInclude(categoty => categoty.ParentCategory)
                                  .Include(product => product.Images)
                                  .ToList();
            return products;
        }

        public async Task<Product> HideProduct(string productId)
        {
            Product productDb = await this.dbContext.Products
                                .FirstOrDefaultAsync(product => product.Id == productId);

            if(productDb == null)
            {
                return productDb;
            }
            productDb.IsHide = true;
            return productDb;
        }
        
        public async Task<bool> ProductExist(string productId)
        {
            bool isContains = await this.dbContext.Products.AnyAsync(product => product.Id == productId);

            return isContains;
        }

        public async Task<Product> ShowProduct(string productId)
        {
            Product productDb = await this.dbContext.Products
                                .FirstOrDefaultAsync(product => product.Id == productId);

            if (productDb == null)
            {
                return productDb;
            }

            productDb.IsHide = false;
            return productDb;
        }

        public int AddImageUrls(string productId, IEnumerable<string> imageUrls)
        {
            Product productDb = this.dbContext.Products
                .Include(product => product.Images)
                .FirstOrDefault(product => product.Id == productId);

            if (productDb == null || imageUrls.Count() == 0)
            {
                return 0;
            }
            
            foreach (var url in imageUrls)
            {
                var image = new ProductImage
                {
                    ImageUrl = url
                };
                productDb.Images.Add(image);
            }
            
            return this.dbContext.SaveChanges();
        }

        public IEnumerable<ProductImage> GetImages(string productId)
        {
            Product productDb = this.dbContext.Products.FirstOrDefault(product => product.Id == productId);

            if (productDb == null)
            {
                return null;
            }

            return productDb.Images.ToList();
        }

        public async Task<bool> EditProduct(Product product)
        {
            if (!await this.ProductExist(product.Id))
            {
                return false;
            }

            try
            {
                this.dbContext.Update(product);
                this.dbContext.SaveChanges();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public IEnumerable<Product> GetProductsByFilter(string type, string value,SubCategory category)
        {
            if (string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            //Take out
            string typeClean = type.Trim().ToLower();
            string valueClean = value.Trim().ToLower();

            switch (typeClean)
            {
                case "brand":
                    typeClean = "Производител";
                break;
                case "ram":
                    typeClean = "Рам";
                break;
                case "cpu":
                    typeClean = "Процесор";
                break;
                case "display":
                    typeClean = "Екран";
                    break;
                default:
                    break;
            }

            string patern = $"{typeClean}:{valueClean}";

            List<Product> products = this.dbContext.Products
                .Include(p => p.Images)
                .Include(p => p.SubCategory)
                .Where(p => p.IsHide == false 
                        && (p.Specification.ToLower().Contains(patern))
                        && (p.SubCategory.Name == category.Name))
                .ToList();

            return products;
        }

        public IEnumerable<Product> GetProductsByFilterPrice(decimal minPrice, decimal maxPrice, SubCategory category)
        {
            var products = this.dbContext.Products
                .Include(p => p.SubCategory)
                .Include(p => p.Images)
                .Where(p => p.IsHide == false &&
                        (p.Price >= minPrice && p.Price <= maxPrice) && p.SubCategory.Name == category.Name)
                .ToList();

            return products;
        }

        public IEnumerable<Product> GetProductsByFilterBrand(string brand, SubCategory category)
        {
            var products = this.dbContext.Products
                .Include(p => p.SubCategory)
                .Include(p => p.Images)
                .Where(p => p.IsHide == false &&
                        (p.Name.Contains(brand)) && p.SubCategory.Name == category.Name)
                .ToList();

            return products;
        }

        //scrapping
        public IEnumerable<Product> GetProductsByFilterPriceOrder(string typeOrder,SubCategory category)
        {
            var products = this.dbContext.Products
                .Include(p => p.SubCategory)
                .Include(p => p.Images)
                .Where(p => p.IsHide == false && p.SubCategory.Name == category.Name)
                .OrderBy(x => x.Price);

            if (typeOrder == "Order")
            {
                products.OrderBy(x => x.Price).ToList();
            }
            else if(typeOrder == "Descending")
            {
                products.OrderByDescending(x => x.Price).ToList();
            }

            return products;
        }
        
        public IEnumerable<Product> OrderBy(IEnumerable<Product> products, string typeOrder)
        {
            if (typeOrder == "Order")
            {
                return products.OrderBy(x => x.Price).ToList();
            }
            else if (typeOrder == "Descending")
            {
                return products.OrderByDescending(x => x.Price).ToList();
            }

            return products.ToList();
        }

        public IEnumerable<Product> GetProductsBySearch(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return new List<Product>();
            }

            string[] searchStringClean = searchString.Split(new string[] { ",", ".", " " }, StringSplitOptions.RemoveEmptyEntries);

            var products = this.dbContext.Products.Include(p => p.SubCategory)
                                                .ThenInclude(c => c.ParentCategory)
                                                .Include(p => p.Images)
                                                .Where(p => p.IsHide == false &&
                                                    searchStringClean.All(c => p.Name.ToLower().Contains(c.ToLower())))
                                                .ToList();
            return products;
        }
        
    }
}
