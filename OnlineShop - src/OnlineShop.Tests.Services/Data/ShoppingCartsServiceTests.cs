using Moq;
using OnlineShop.Models;
using OnlineShop.Service.Data;
using OnlineShop.Service.Data.Contracts;
using OnlineShop.Tests.Services.Common;
using OnlineShop.Web.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShop.Tests.Services.Data
{
    public class ShoppingCartsServiceTests
    {
        private OnlineShopDbContext context;
        private IShoppingCartsService shoppingCartsService;
        private Mock<IProductService> mockProductService;
        private Mock<IUserService> mockUserService;
    
        public ShoppingCartsServiceTests()
        {
            this.context = OnlineShopDbContextInMemoryFactory.InitializeContext();
            this.mockProductService = new Mock<IProductService>();
            this.mockUserService = new Mock<IUserService>();

            this.shoppingCartsService = new ShoppingCartsService(context, 
                                mockProductService.Object, mockUserService.Object);
        }

        [Fact]
        public async Task AddProductInShoppingCart_ShouldAddProduct()
        {
            //Arrange
            ShopUser user = new ShopUser
            {
                UserName = "user@abv.bg",
                ShoppingCart = new ShoppingCart()
            };

            Product product = new Product { Name = "TV box" };
            
            await this.context.Users.AddAsync(user);
            await this.context.SaveChangesAsync();

            string productId = "234f-6345-t3357-234";
            int quantity = 2;
            this.mockUserService.Setup(x => x.GetUserByUsername(user.UserName)).Returns(user);
            this.mockProductService.Setup(x => x.GetProductById(productId))
                                    .Returns(Task.FromResult<Product>(product));

            //Act
            await this.shoppingCartsService.AddProductInShoppingCart(productId, user.UserName, quantity);
            
            //Assert
            Assert.NotEmpty(this.context.ShoppingCartProducts);
        }

        [Theory]
        [InlineData("","",0)]
        [InlineData("  ", "   ",-1241)]
        public async Task AddProductInShoppingCart_WithInvalidParameters_ShouldNotAddProduct(string id,string username,int quantity)
        {
            //Arrange

            //Act
            await this.shoppingCartsService.AddProductInShoppingCart(id,username,quantity);

            //Assert
            Assert.Empty(this.context.ShoppingCartProducts);
        }

        [Fact]
        public async Task AnyProducts_ShouldReturnTrue()
        {
            //Arrange
            ShopUser user = new ShopUser
            {
                UserName = "user@gmail.com",
                ShoppingCart = new ShoppingCart()
            };
            await this.context.Users.AddAsync(user);

            Product product = new Product { Name = "USB Cable" };
            await this.context.Products.AddAsync(product);
            await this.context.SaveChangesAsync();
            
            this.mockUserService.Setup(r => r.GetUserByUsername(user.UserName))
                       .Returns(user);
            
            this.mockProductService.Setup(p => p.GetProductById(product.Id))
                          .Returns(Task.FromResult<Product>(product));

            await shoppingCartsService.AddProductInShoppingCart(product.Id, user.UserName);

            //Act
            bool areAnyProducts = shoppingCartsService.AnyProducts(user.UserName);

            //Assert
            Assert.True(areAnyProducts);
        }

        [Fact]
        public void AnyProducts_WithInvalidUsername_ShouldReturnFalse()
        {
            //Arrange
            string username = "gosho";

            //Act
            bool areAnyProducts = shoppingCartsService.AnyProducts(username);

            //Assert
            Assert.False(areAnyProducts);
        }

        [Fact]
        public void DeleteAllProductFromShoppingCart_ShouldDeleteProducts()
        {
            //Arrange
            List<Product> products = new List<Product>
            {
               new Product { Name = "USB 1.0" },
               new Product { Name = "USB 2.0" },
               new Product { Name = "USB 3.0" },
               new Product { Name = "USB 4.0" }
            };
            this.context.Products.AddRange(products);

            var shoppingCartProducts = new List<ShoppingCartProduct>
            {
               new ShoppingCartProduct { Product = products.First() },
               new ShoppingCartProduct { Product = products.Last() },
            };

            ShopUser user = new ShopUser
            {
                UserName = "user@gmail.com",
                ShoppingCart = new ShoppingCart
                {
                    ShoppingCartProducts = shoppingCartProducts
                }
            };
            this.context.Users.Add(user);
            this.context.SaveChanges();

            this.mockUserService.Setup(r => r.GetUserByUsername(user.UserName))
                       .Returns(user);
            
            //Act
            int result = this.shoppingCartsService.DeleteAllProductFromShoppingCart(user.UserName);

            //Assert
            Assert.True(result > 0);
            Assert.Empty(user.ShoppingCart.ShoppingCartProducts);
        }

        [Fact]
        public void DeleteAllProductFromShoppingCart_WithInvalidUsername_ShouldNoChange()
        {
            //Arrange
            string username = "roncho";

            //Act
            int result = this.shoppingCartsService.DeleteAllProductFromShoppingCart(username);

            //Assert
            Assert.False(result > 0);
        }

        [Fact]
        public async Task DeleteProductFromShoppingCart_ShouldDeleteProduct()
        {
            //Arrange
            Product deleteProduct = new Product { Name = "DVD Player" };

            var shoppingCartProducts = new List<ShoppingCartProduct>
            {
               new ShoppingCartProduct { Product = deleteProduct },
               new ShoppingCartProduct { Product = new Product { Name = "Aux Cable" }, },
            };

            var user = new ShopUser
            {
                UserName = "user@gmail.com",
                ShoppingCart = new ShoppingCart
                {
                    ShoppingCartProducts = shoppingCartProducts
                }
            };
            await this.context.Users.AddAsync(user);
            await this.context.SaveChangesAsync();
            
            this.mockUserService.Setup(r => r.GetUserByUsername(user.UserName))
                       .Returns(user);

            this.mockProductService.Setup(p => p.GetProductById(deleteProduct.Id))
                          .Returns(Task.FromResult<Product>(deleteProduct));
            
            //Act
            int result = await shoppingCartsService.DeleteProductFromShoppingCart(deleteProduct.Id, user.UserName);

            //Assert
            Assert.True(result > 0);
            Assert.Single(shoppingCartProducts);
        }
        
        [Theory]
        [InlineData("","")]
        [InlineData("   ", "    ")]
        public async Task DeleteProductFromShoppingCart_WithInvalidParameters_ShouldDeleteProduct(string productId, string username)
        {
            //Arrange

            //Act
            int result = await shoppingCartsService.DeleteProductFromShoppingCart(productId, username);

            //Assert
            Assert.False(result > 0);
        }

        [Fact]
        public async Task EditProductQuantityInShoppingCart_ShouldEditProduct()
        {
            //Arrange
            Product editProduct = new Product { Name = "DVD Player" };
            int quantity = 5;

            var shoppingCartProducts = new List<ShoppingCartProduct>
            {
               new ShoppingCartProduct { Product = editProduct ,Quantity = 3 },
               new ShoppingCartProduct { Product = new Product { Name = "Aux Cable" }, Quantity = 2 },
            };

            var user = new ShopUser
            {
                UserName = "user@gmail.com",
                ShoppingCart = new ShoppingCart
                {
                    ShoppingCartProducts = shoppingCartProducts
                }
            };
            await this.context.Users.AddAsync(user);
            await this.context.SaveChangesAsync();

            this.mockUserService.Setup(r => r.GetUserByUsername(user.UserName))
                       .Returns(user);

            this.mockProductService.Setup(p => p.GetProductById(editProduct.Id))
                          .Returns(Task.FromResult<Product>(editProduct));

            //Act
            int result = await shoppingCartsService.EditProductQuantityInShoppingCart(editProduct.Id, user.UserName,quantity);

            //Assert
            Assert.True(result > 0);
            Assert.Equal(quantity,shoppingCartProducts.First().Quantity);
        }

        [Theory]
        [InlineData("", "",1)]
        [InlineData("   ", "    ",-12)]
        [InlineData("   ", "    ", 0)]
        public async Task EditProductQuantityInShoppingCart_WithInvalidParameters_ShouldDeleteProduct(string productId, string username, int quantity)
        {
            //Arrange

            //Act
            int result = await shoppingCartsService.EditProductQuantityInShoppingCart(productId, username,quantity);

            //Assert
            Assert.False(result > 0);
        }

        [Fact]
        public void GetAllShoppingCartProducts_ShouldReturnCollection()
        {
            //Arrange
            var shoppingCartProducts = new List<ShoppingCartProduct>
            {
               new ShoppingCartProduct { Product =  new Product { Name = "TV box" }, },
               new ShoppingCartProduct { Product =  new Product { Name = "Mini PC HP" }, },
               new ShoppingCartProduct { Product =  new Product { Name = "Laptop Lenovo" }, },
               new ShoppingCartProduct { Product =  new Product { Name = "Lan Cable" }, },
            };

            ShopUser user = new ShopUser
            {
                UserName = "user@gmail.com",
                ShoppingCart = new ShoppingCart
                {
                    ShoppingCartProducts = shoppingCartProducts
                }
            };

            this.context.Users.Add(user);
            this.context.SaveChanges();

            this.mockUserService.Setup(r => r.GetUserByUsername(user.UserName))
                       .Returns(user);

            //Act
            var actualCollection = this.shoppingCartsService.GetAllShoppingCartProducts(user.UserName);

            //Assert
            Assert.NotEmpty(actualCollection);
            Assert.Equal(shoppingCartProducts.Count,actualCollection.Count());
        }

        [Fact]
        public void GetAllShoppingCartProducts_WithInvalidUsername_ShouldReturnCollection()
        {
            //Arrange
            string username = "roncho";

            //Act
            var collection = this.shoppingCartsService.GetAllShoppingCartProducts(username);

            //Assert
            Assert.Empty(collection);
        }

    }
}
