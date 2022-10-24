using Microsoft.AspNetCore.Identity;
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

    public class UserServiceTests
    {
        private IUserService userService;
        
        private List<ShopUser> GetDummyData()
        {
            return new List<ShopUser>()
            {
                new ShopUser
                {
                    UserName = "Tosho",
                    Company = new Company
                    {
                         Name = "Eko hidro"
                    }
                },
                new ShopUser
                {
                    UserName = "Ivan",
                    Company = new Company
                    {
                         Name = "Techcompany EOOD"
                    }
                },
            };
        }

        private async Task SeedData(OnlineShopDbContext context)
        {
            context.AddRange(GetDummyData());
            await context.SaveChangesAsync();
        }

        [Fact]
        public void GetUserByUsername_ShouldReturnCorrect()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            var mockUserStore = new Mock<IUserStore<ShopUser>>();
            var mockUserManager = new Mock<UserManager<ShopUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            ShopUser user = new ShopUser
            {
                UserName = "test@abv.bg"
            };

            mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);

            userService = new UserService(mockUserManager.Object, context);
            
            context.Users.Add(user);
            context.SaveChanges();

            //Act
            ShopUser actualUser = userService.GetUserByUsername(user.UserName);

            //Assert
            Assert.NotNull(actualUser);
            Assert.Equal(user.UserName,actualUser.UserName);
        }

        [Fact]
        public void GetUserByUsername_WithEmptyUsername_ShouldReturnNull()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            var mockUserStore = new Mock<IUserStore<ShopUser>>();
            var mockUserManager = new Mock<UserManager<ShopUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);
            
            userService = new UserService(mockUserManager.Object, context);
            
            //Act
            ShopUser actualUser = userService.GetUserByUsername("");

            //Assert
            Assert.Null(actualUser);
        }

        [Fact]
        public async Task GetUserCompanyByUsername_ShouldReturnCorrect()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            var mockUserStore = new Mock<IUserStore<ShopUser>>();
            var mockUserManager = new Mock<UserManager<ShopUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            userService = new UserService(mockUserManager.Object, context);
            
            await SeedData(context);

            //Act
            Company company = userService.GetUserCompanyByUsername("Ivan");

            //Assert
            Assert.NotNull(company);
            Assert.Equal("Techcompany EOOD", company.Name);
        }

        [Fact]
        public async Task CreateCompany_ShouldBeOk()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            var mockUserStore = new Mock<IUserStore<ShopUser>>();
            var mockUserManager = new Mock<UserManager<ShopUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            ShopUser user = new ShopUser
            {
                UserName = "test@abv.bg"
            };

            context.Users.Add(user);
            context.SaveChanges();

            mockUserManager.Setup(m => m.FindByNameAsync(user.UserName))
                        .Returns(Task.FromResult<ShopUser>(user));
            
            userService = new UserService(mockUserManager.Object, context);
            
            Company company = new Company
            {
                Name = "Emag",
                Owner = "Gosho"
            };

            //Act
            int result = await userService.CreateCompany(company, user.UserName);

            var actualUser = userService.GetUserByUsername(user.UserName);

            //Assert
            Assert.True(result > 0);
            Assert.Equal(company.Name, actualUser.Company.Name);
        }

        [Fact]
        public async Task CreateCompany_WithInvalidUsername_DoNotCreateCompany()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            var mockUserStore = new Mock<IUserStore<ShopUser>>();
            var mockUserManager = new Mock<UserManager<ShopUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            ShopUser user = new ShopUser
            {
                UserName = "test@abv.bg"
            };
            
            mockUserManager.Setup(m => m.FindByNameAsync(user.UserName))
                        .Returns(Task.FromResult<ShopUser>(user));

            userService = new UserService(mockUserManager.Object, context);

            Company company = new Company
            {
                Name = "Emag",
                Owner = "Gosho"
            };

            //Act
            int result = await userService.CreateCompany(company, user.UserName);
            
            //Assert
            Assert.False(result > 0);
        }

        [Fact]
        public async Task CreateCompany_CompanyNull_DoNotCreateCompany()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            var mockUserStore = new Mock<IUserStore<ShopUser>>();
            var mockUserManager = new Mock<UserManager<ShopUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            ShopUser user = new ShopUser
            {
                UserName = "test@abv.bg"
            };
            
            mockUserManager.Setup(m => m.FindByNameAsync(user.UserName))
                        .Returns(Task.FromResult<ShopUser>(user));

            userService = new UserService(mockUserManager.Object, context);

            Company company = null;

            //Act
            int result = await userService.CreateCompany(company, user.UserName);

            //Assert
            Assert.False(result > 0);
        }

        [Fact]
        public async Task AddUserToRole_ShouldReturnTrue()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();
     
            string username = "Daniel";
            string role = "Admin";

            var users = new List<ShopUser>
            {
                new ShopUser{UserName = "user1",},
                new ShopUser{UserName = "user2",},
                new ShopUser{UserName = username,}
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();

            var mockUserStore = new Mock<IUserStore<ShopUser>>();
            var mockUserManager = new Mock<UserManager<ShopUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            mockUserManager.Setup(m => m.FindByNameAsync(username))
                        .Returns(Task.FromResult<ShopUser>(users.FirstOrDefault(x => x.UserName == username)));

            userService = new UserService(mockUserManager.Object, context);

            //Act
            bool isUserAddInRole = userService.AddUserToRole(username,role);

            //Assert
            Assert.True(isUserAddInRole);
        }

        [Fact]
        public void AddUserToRole_WithInvalidUser_ShouldReturnFalse()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            string username = "Daniel";
            string role = "Admin";

            ShopUser user = null; 
            var mockUserStore = new Mock<IUserStore<ShopUser>>();
            var mockUserManager = new Mock<UserManager<ShopUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            mockUserManager.Setup(m => m.FindByNameAsync(username))
                        .Returns(Task.FromResult<ShopUser>(user));

            userService = new UserService(mockUserManager.Object, context);

            //Act
            bool isUserAddInRole = userService.AddUserToRole(username, role);

            //Assert
            Assert.False(isUserAddInRole);
        }

        [Fact]
        public async Task RemoveUserFromRole_ShouldReturnTrue()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();
            
            var user = new ShopUser
            {
                UserName = "shop@gmail.com"
            };

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            var role = new IdentityRole { Name = "Admin" };
            await context.Roles.AddAsync(role);
            await context.UserRoles.AddAsync(new IdentityUserRole<string> { UserId = user.Id, RoleId = role.Id });
            await context.SaveChangesAsync();

            var mockUserStore = new Mock<IUserStore<ShopUser>>();
            var mockUserManager = new Mock<UserManager<ShopUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            mockUserManager.Setup(m => m.FindByNameAsync(user.UserName))
                        .Returns(Task.FromResult<ShopUser>(user));

            userService = new UserService(mockUserManager.Object, context);

            //Act
            bool isUserAddInRole = userService.RemoveUserFromRole(user.UserName, role.Name);

            //Assert
            Assert.True(isUserAddInRole);
        }

        [Fact]
        public void RemoveUserFromRole_WithInvalidUser_ShouldReturnFalse()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            ShopUser user = null;
            string username = "Dani";
            string role ="Admin";

            var mockUserStore = new Mock<IUserStore<ShopUser>>();
            var mockUserManager = new Mock<UserManager<ShopUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            mockUserManager.Setup(m => m.FindByNameAsync(username))
                        .Returns(Task.FromResult<ShopUser>(user));

            userService = new UserService(mockUserManager.Object, context);

            //Act
            bool isUserAddInRole = userService.RemoveUserFromRole(username, role);

            //Assert
            Assert.False(isUserAddInRole);
        }

        [Fact]
        public async Task EditFirstName_ShouldReturnTrue()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();
            string newFirstName = "Drago";

            var user = new ShopUser
            {
                UserName = "shop@gmail.com",
                FirstName = "Staso"
            };

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            
            var mockUserStore = new Mock<IUserStore<ShopUser>>();
            var mockUserManager = new Mock<UserManager<ShopUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);
            
            userService = new UserService(mockUserManager.Object, context);

            //Act
            bool isEdit = userService.EditFirstName(user, newFirstName);
            
            //Assert
            Assert.True(isEdit);
            Assert.Equal(newFirstName, user.FirstName);
        }

        [Fact]
        public void EditFirstName_WithUserIsNull_ShouldReturnFalse()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();
            string newFirstName = "Drago";

            ShopUser user = null;
            
            var mockUserStore = new Mock<IUserStore<ShopUser>>();
            var mockUserManager = new Mock<UserManager<ShopUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            userService = new UserService(mockUserManager.Object, context);

            //Act
            bool isEdit = userService.EditFirstName(user, newFirstName);

            //Assert
            Assert.False(isEdit);
        }

        [Fact]
        public async Task EditFirstName_WithFistNameIsNull_ShouldReturnFalse()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();
            string newFirstName = "";

            var user = new ShopUser
            {
                UserName = "shop@gmail.com",
                FirstName = "Staso"
            };

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            var mockUserStore = new Mock<IUserStore<ShopUser>>();
            var mockUserManager = new Mock<UserManager<ShopUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            userService = new UserService(mockUserManager.Object, context);

            //Act
            bool isEdit = userService.EditFirstName(user, newFirstName);

            //Assert
            Assert.False(isEdit);
        }
        
        [Fact]
        public async Task EditLastName_ShouldReturnTrue()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();
            string newLastName = "Drago";

            var user = new ShopUser
            {
                UserName = "shop@gmail.com",
                LastName = "Staso"
            };

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            var mockUserStore = new Mock<IUserStore<ShopUser>>();
            var mockUserManager = new Mock<UserManager<ShopUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            userService = new UserService(mockUserManager.Object, context);

            //Act
            bool isEdit = userService.EditLastName(user, newLastName);

            //Assert
            Assert.True(isEdit);
            Assert.Equal(newLastName, user.LastName);
        }

        [Fact]
        public void EditLastName_WithUserIsNull_ShouldReturnFalse()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();
            string newLastName = "Drago";

            ShopUser user = null;

            var mockUserStore = new Mock<IUserStore<ShopUser>>();
            var mockUserManager = new Mock<UserManager<ShopUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            userService = new UserService(mockUserManager.Object, context);

            //Act
            bool isEdit = userService.EditLastName(user, newLastName);

            //Assert
            Assert.False(isEdit);
        }

        [Fact]
        public async Task EditLastName_WithLastNameIsNull_ShouldReturnFalse()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();
            string newLastName = "";

            var user = new ShopUser
            {
                UserName = "shop@gmail.com",
                LastName = "Staso"
            };

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            var mockUserStore = new Mock<IUserStore<ShopUser>>();
            var mockUserManager = new Mock<UserManager<ShopUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            userService = new UserService(mockUserManager.Object, context);

            //Act
            bool isEdit = userService.EditLastName(user, newLastName);

            //Assert
            Assert.False(isEdit);
        }

        [Fact]
        public async Task GetUserCompanyByUsername_ShouldReturnCompany()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();
            
            Company expectedCompany = new Company
            {
                Name = "testCompany",
                ShopUser = new ShopUser
                {
                    UserName = "test@abv.bg"
                },
                Address = new Address
                {
                    Country = "Bulgaria",
                    City = new City
                    {
                        Name = "Dragor"
                    }
                }
            };

            await context.Companies.AddAsync(expectedCompany);
            await context.SaveChangesAsync();

            var mockUserStore = new Mock<IUserStore<ShopUser>>();
            var mockUserManager = new Mock<UserManager<ShopUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            userService = new UserService(mockUserManager.Object, context);

            //Act
            Company actualCompany = userService.GetUserCompanyByUsername(expectedCompany.ShopUser.UserName);

            //Assert
            Assert.NotNull(actualCompany);
            Assert.Equal(expectedCompany.Name, actualCompany.Name);
            Assert.Equal(expectedCompany.ShopUser.UserName, actualCompany.ShopUser.UserName);
            Assert.Equal(expectedCompany.Address.Country, actualCompany.Address.Country);
            Assert.Equal(expectedCompany.Address.City, actualCompany.Address.City);
        }

        [Fact]
        public async Task GetUserCompanyByUsername_WithInvalidUsername_ShouldReturnNull()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            var mockUserStore = new Mock<IUserStore<ShopUser>>();
            var mockUserManager = new Mock<UserManager<ShopUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            userService = new UserService(mockUserManager.Object, context);

            await SeedData(context);

            //Act
            Company company = userService.GetUserCompanyByUsername("Drago");

            //Assert
            Assert.Null(company);
        }
        
    }
}
