using Moq;
using OnlineShop.Service.Data.Contracts;
using OnlineShop.Web.Data;
using System;
using System.Collections.Generic;
using System.Text;
using OnlineShop.Tests.Services.Common;
using OnlineShop.Service.Data;
using Xunit;
using OnlineShop.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace OnlineShop.Tests.Services.Data
{
    public class FavoriteServiceTests
    {
        private IFavoriteService favoriteService;
        private Mock<IUserService> mockUserService;
        private OnlineShopDbContext context;
        
        public FavoriteServiceTests()
        {
            this.context = OnlineShopDbContextInMemoryFactory.InitializeContext();
            this.mockUserService = new Mock<IUserService>();
            this.favoriteService = new FavoriteService(context, mockUserService.Object);
        }

        [Fact]
        public void Add_ShouldReturnTrue()
        {
            //Arrange
            ShopUser user = new ShopUser
            {
                UserName = "user@abv.bg",
                FavoriteProducts = new List<ShopUserFavoriteProduct>()
            };
            Product product = new Product { Name = "Razer Headphone" };

            this.context.Users.Add(user);
            this.context.Products.Add(product);
            this.context.SaveChanges();
            
            this.mockUserService.Setup(x => x.GetUserByUsernameWithFavoriteProduct(user.UserName))
                        .Returns(user);

            //Act
            bool isAdded = this.favoriteService.Add(product.Id, user.UserName);
            
            //Assert
            Assert.True(isAdded);
            Assert.Single(user.FavoriteProducts);
        }

        [Theory]
        [InlineData("","")]
        [InlineData(" ", " ")]
        public void Add_WithInvalidParameter_ShouldReturnFalse(string productId, string username)
        {
            //Arrange
            
            //Act
            bool isAdded = this.favoriteService.Add(productId, username);

            //Assert
            Assert.False(isAdded);
        }

        [Fact]
        public void AllProducts_ShouldReturnCollection()
        {
            //Arrange
            var favoriteProducts = new List<ShopUserFavoriteProduct>
            {
                new ShopUserFavoriteProduct { Product = new Product { Name = "RCA Cable" }},
                new ShopUserFavoriteProduct { Product = new Product { Name = "HDMI Cable" }},
                new ShopUserFavoriteProduct { Product = new Product { Name = "VGA Cable" }},
            };

            ShopUser user = new ShopUser
            {
                UserName = "user@abv.bg",
                FavoriteProducts = favoriteProducts
            };
            
            this.context.Users.Add(user);
            this.context.SaveChanges();

            //Act
            var collection = this.favoriteService.AllProducts(user.UserName);

            //Assert
            Assert.NotEmpty(collection);
            Assert.Equal(favoriteProducts.Count,user.FavoriteProducts.Count);
        }

        [Fact]
        public void Delete_ShouldDeleteProduct()
        {
            //Arrange
            Product deleteProduct = new Product { Name = "RCA Cable" };

            var favoriteProducts = new List<ShopUserFavoriteProduct>
            {
                new ShopUserFavoriteProduct { Product = deleteProduct},
                new ShopUserFavoriteProduct { Product = new Product { Name = "HDMI Cable" }},
                new ShopUserFavoriteProduct { Product = new Product { Name = "VGA Cable" }},
            };

            ShopUser user = new ShopUser
            {
                UserName = "user@abv.bg",
                FavoriteProducts = favoriteProducts
            };

            this.context.Users.Add(user);
            this.context.SaveChanges();

            //Act
            int result = this.favoriteService.Delete(deleteProduct.Id,user.UserName);

            //Assert
            Assert.True(result > 0);
            Assert.Equal(2, user.FavoriteProducts.Count);
        }
    }
}
