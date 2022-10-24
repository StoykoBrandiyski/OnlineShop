using Moq;
using OnlineShop.Models;
using OnlineShop.Service.Data;
using OnlineShop.Service.Data.Contracts;
using OnlineShop.Tests.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OnlineShop.Tests.Services.Data
{
    public class AddressServiceTests
    {
        private IAddresService addresService;

        [Fact]
        public async Task CreateAddress_ShouldBeOk()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            this.addresService = new AddressService(context, null);

            Address exAddress = new Address
            {
                Street = "Todor Kableshkov 10",
                Description = "This is a wonderful location",
                Country = "Bulgaria",
                City = new City
                {
                    Name = "Plovdiv",
                    Postcode = "4400"
                }
            };

            //Act
            Address actualAddress = await this.addresService.CreateAddress(exAddress.Street,
                exAddress.Country,
                exAddress.Description,
                exAddress.City.Name,
                exAddress.City.Postcode);

            //Assert
            Assert.NotNull(actualAddress);
            Assert.Equal(1, context.Addresses.Count());
            Assert.Equal(exAddress.Street, actualAddress.Street);
            Assert.Equal(exAddress.Country, actualAddress.Country);
            Assert.Equal(exAddress.Description, actualAddress.Description);
            Assert.Equal(exAddress.City.Name, actualAddress.City.Name);
            Assert.Equal(exAddress.City.Postcode, actualAddress.City.Postcode);

        }

        [Theory]
        [InlineData("","","","","")]
        [InlineData("    ","   ","   ","     ","     ")]
        public async Task CreateAddress_WithEmptyParameters_ReturnNull(string street,string country, string description,string city,string postcode)
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            this.addresService = new AddressService(context, null);

            //Act
            Address actualAddress = await this.addresService.CreateAddress(street, country, description, city, postcode);

            //Assert
            Assert.Null(actualAddress);
            Assert.Empty(context.Addresses);
            Assert.Empty(context.Cities);

        }

        [Fact]
        public async Task AddAddressToUser_ShouldBeOk()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            string username = "Gataka";
            ShopUser user = new ShopUser { UserName = username };

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            Address exAddress = new Address
            {
                Street = "Todor Kableshkov 10",
                Description = "This is a wonderful location",
                City = new City
                {
                    Name = "Plovdiv",
                    Postcode = "4400"
                }
            };
            
            var mockUserService = new Mock<IUserService>();

            mockUserService.Setup(x => x.GetUserByUsername(username)).Returns(user);

            this.addresService = new AddressService(context, mockUserService.Object);

            //Act
            int result = this.addresService.AddAddressToUser(username, exAddress);

            Address actualAddress = context.Users
                .FirstOrDefault(x => x.UserName == username)
                .Addresses.First();
     
            //Assert
            Assert.True(result > 0);
            Assert.Equal(exAddress.Street, actualAddress.Street);
            Assert.Equal(exAddress.Description, actualAddress.Description);
            Assert.Equal(exAddress.City.Name, actualAddress.City.Name);
            Assert.Equal(exAddress.City.Postcode, actualAddress.City.Postcode);
        }

        [Fact]
        public void AddAddressToUser_WithInvalidUsername_NotAddAddress()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            string username = "Gataka";

            Address exAddress = new Address
            {
                Street = "Todor Kableshkov 10",
                Description = "This is a wonderful location",
                City = new City
                {
                    Name = "Plovdiv",
                    Postcode = "4400"
                }
            };

            var mockUserService = new Mock<IUserService>();
            
            this.addresService = new AddressService(context, mockUserService.Object);

            //Act
            int result = this.addresService.AddAddressToUser(username, exAddress);
            
            //Assert
            Assert.False(result > 0);
        }

        [Fact]
        public void AddAddressToUser_WithAddresssNull_NotAddAddress()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            string username = "Gataka";

            Address exAddress = null;

            var mockUserService = new Mock<IUserService>();

            this.addresService = new AddressService(context, mockUserService.Object);

            //Act & Assert
            Assert.Throws<ArgumentNullException>(() => this.addresService.AddAddressToUser(username, exAddress));
            
        }

        [Fact]
        public async Task GetAllUserAddress_ShouldBeOk()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            string username = "Gataka";
            ShopUser user = new ShopUser { UserName = username };

            City city = new City { Name = "Plovdiv", Postcode = "4400" };

            var Address = new List<Address>
            {
                new Address
                {
                    Street = "Todor Kableshkov 10",
                    Description = "This is a wonderful location",
                    City = city,
                    ShopUser = user
                },
                new Address
                {
                    Street = "Todor Kableshkov 10",
                    Description = "This is a wonderful location",
                    City = city,
                    ShopUser = user
                },
            };

            await context.Users.AddAsync(user);
            await context.Addresses.AddRangeAsync(Address);
            await context.SaveChangesAsync();

            var mockUserService = new Mock<IUserService>();

            mockUserService.Setup(x => x.GetUserByUsername(username)).Returns(user);

            this.addresService = new AddressService(context, mockUserService.Object);

            //Act
            var userAddress = this.addresService.GetAllUserAddress(username);

            //Assert
            Assert.NotEmpty(userAddress);
            Assert.Equal(Address.Count, userAddress.Count());

            foreach (var item in userAddress)
            {
                Assert.Equal(item.ShopUser.UserName, username);
            }
        }

        [Fact]
        public void GetAllUserAddress_WithInvalidUsername_NotAddAddress()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            string username = "Gataka";
            
            var mockUserService = new Mock<IUserService>();

            this.addresService = new AddressService(context, mockUserService.Object);

            //Act
            var userAddress = this.addresService.GetAllUserAddress(username);

            //Assert
            Assert.Empty(userAddress);
        }
    }
}
