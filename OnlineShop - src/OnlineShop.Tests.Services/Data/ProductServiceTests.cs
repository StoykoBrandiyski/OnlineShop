using OnlineShop.Models;
using OnlineShop.Service.Data;
using OnlineShop.Service.Data.Contracts;
using OnlineShop.Tests.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShop.Tests.Services.Data
{
    public class ProductServiceTests
    {
        private IProductService productService;

        [Fact]
        public async Task AddProduct_ShouldBeOk()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            Product product = new Product
            {
                Name = "Samsung Galaxy S7",
                SubCategory = new SubCategory
                {
                    Name = "smartphone"
                },
                Description = "The best smartphone for 2018 year."
            };

            productService = new ProductService(context);

            //Act 
            int result = await productService.AddProduct(product);
            
            //Assert
            Assert.True(result > 0);
            Assert.Equal(1, context.Products.Count());
        }

        [Fact]
        public async Task AddProduct_WithProductNull_Throw()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            Product product = null;

            productService = new ProductService(context);

            //Act & Assert 
            await Assert.ThrowsAsync<ArgumentNullException>(() => productService.AddProduct(product));

        }

        [Fact]
        public async Task GetAllProductByCategoryId_ShouldReturnProducts()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            string categoryName = "smartphone";

            SubCategory categoryProduct = new SubCategory { Name = categoryName };

            var products = new List<Product>
            {
                new Product { Name = "Samsung", SubCategory = categoryProduct},
                new Product { Name = "Nokia", SubCategory = categoryProduct},
                new Product { Name = "Apple", SubCategory = categoryProduct},
                new Product { Name = "Xiaomi", SubCategory = categoryProduct},
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();

            int categoryId = context.SubCategories.First().Id;

            productService = new ProductService(context);

            //Act 
            var actualProducts = await productService.GetAllProductByCategoryId(categoryId);

            //Assert
            Assert.NotEmpty(actualProducts);
            Assert.Equal(4, actualProducts.Count());

            foreach (var item in actualProducts)
            {
                Assert.Equal(item.SubCategory.Name, categoryName);
            }
        }

        [Fact]
        public async Task GetAllProductByCategoryId_WithInvalidCategory_ShouldReturnEmptyCollection()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();


            SubCategory categoryProduct = new SubCategory { Name = "auto" };

            var products = new List<Product>
            {
                new Product { Name = "Samsung", SubCategory =  new SubCategory { Name = "smartphone" } },
                new Product { Name = "Hp", SubCategory =  new SubCategory { Name = "Computer" } },
            };

            await context.SubCategories.AddAsync(categoryProduct);
            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();

            int categoryId = 1;

            productService = new ProductService(context);

            //Act 
            var actualProducts = await productService.GetAllProductByCategoryId(categoryId);

            //Assert
            Assert.Empty(actualProducts);
        }

        [Fact]
        public async Task GetProductById_ShouldBeOk()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            Product product = new Product
            {
                Name = "Samsung Galaxy S7",
                SubCategory = new SubCategory
                {
                    Name = "smartphone"
                },
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "storage/car.jpg"},
                    new ProductImage { ImageUrl = "storage/car2.jpg"},
                },
                 
                Description = "The best smartphone for 2018 year.",
                Price = 260.0M
            };

            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            productService = new ProductService(context);

            string productId = context.Products.First().Id;

            //Act 
            Product actualProduct = await productService.GetProductById(productId);

            //Assert
            Assert.NotNull(actualProduct);
            Assert.Equal(product.Name, actualProduct.Name);
            Assert.Equal(product.Price, actualProduct.Price);
            Assert.NotEmpty(actualProduct.Images);
        }

        [Fact]
        public async Task GetProductById_WithInvalidProductId_ShouldReturnNull()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();
            
            productService = new ProductService(context);

            string productId = "241fdf-3423f-sd34";

            //Act 
            Product actualProduct = await productService.GetProductById(productId);

            //Assert
            Assert.Null(actualProduct);
        }

        [Fact]
        public async Task HideProduct_ShouldReturnProduct()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            Product product = new Product
            {
                Name = "Samsung Galaxy S7",
                SubCategory = new SubCategory
                {
                    Name = "smartphone"
                },
                Description = "The best smartphone for 2018 year.",
                Price = 260.0M
            };

            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            productService = new ProductService(context);

            string productId = context.Products.First().Id;

            //Act 
            Product actualProduct = await productService.HideProduct(productId);
            
            //Assert
            Assert.True(actualProduct.IsHide);
        }

        [Fact]
        public async Task HideProduct_WithInvalidProductId_ShouldReturnNull()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            Product product = new Product
            {
                Name = "Samsung Galaxy S7",
                SubCategory = new SubCategory
                {
                    Name = "smartphone"
                },
                Description = "The best smartphone for 2018 year.",
                Price = 260.0M
            };

            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            productService = new ProductService(context);

            string productId = "12fsd-234dfs-342fgd";

            //Act 
            Product actualProduct = await productService.HideProduct(productId);
            
            //Assert
            Assert.Null(actualProduct);
        }

        [Fact]
        public async Task ProductExist_ShouldReturnTrue()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            Product product = new Product
            {
                Name = "Samsung Galaxy S7",
                SubCategory = new SubCategory
                {
                    Name = "smartphone"
                },
                Description = "The best smartphone for 2018 year.",
                Price = 260.0M
            };

            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            productService = new ProductService(context);

            string productId = context.Products.First().Id;

            //Act 
            bool isExist = await productService.ProductExist(productId);

            //Assert
            Assert.True(isExist);
        }

        [Fact]
        public async Task HideProduct_WithInvalidProductId_ShouldReturnFalse()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            Product product = new Product
            {
                Name = "Samsung Galaxy S7",
                SubCategory = new SubCategory
                {
                    Name = "smartphone"
                },
                Description = "The best smartphone for 2018 year.",
                Price = 260.0M
            };

            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            productService = new ProductService(context);

            string productId = "12fsd-234dfs-342fgd";

            //Act 
            bool isExist = await productService.ProductExist(productId);

            //Assert
            Assert.False(isExist);
        }

        [Fact]
        public async Task ShowProduct_ShouldReturnProduct()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            Product product = new Product
            {
                Name = "Samsung Galaxy S7",
                SubCategory = new SubCategory
                {
                    Name = "smartphone"
                },
                IsHide = true,
                Description = "The best smartphone for 2018 year.",
                Price = 260.0M
            };

            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            productService = new ProductService(context);

            string productId = context.Products.First().Id;

            //Act 
            Product actualProduct = await productService.ShowProduct(productId);

            //Assert
            Assert.NotNull(actualProduct);
            Assert.False(actualProduct.IsHide);
        }

        [Fact]
        public async Task ShowProduct_WithInvalidProductId_ShouldReturnNull()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            Product product = new Product
            {
                Name = "Samsung Galaxy S7",
                SubCategory = new SubCategory
                {
                    Name = "smartphone"
                },
                IsHide = true,
                Description = "The best smartphone for 2018 year.",
                Price = 260.0M
            };

            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            productService = new ProductService(context);

            string productId = "12fsd-234dfs-342fgd";

            //Act 
            Product actualProduct = await productService.HideProduct(productId);

            //Assert
            Assert.Null(actualProduct);
           
        }

    }
}
