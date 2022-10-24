using OnlineShop.Models;
using OnlineShop.Service.Data;
using OnlineShop.Service.Data.Contracts;
using OnlineShop.Tests.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace OnlineShop.Tests.Services.Data
{
    public class ParentCategoryServiceTests
    {
        private IParentCategoryService parentCategoryService;

        [Fact]
        public void CreateCategory_ShouldReturnCategory()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();
            
            this.parentCategoryService = new ParentCategoryService(context);
            string name = "Computers and Tablets";

            //Act
            ParentCategory category = this.parentCategoryService.CreateCategory(name);

            //Assert
            Assert.NotNull(category);
            Assert.NotEmpty(context.ParentCategories);
            Assert.Equal(name, category.Name);
        }

        [Fact]
        public void CreateCategory_WithExistName_ShouldReturnNull()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            this.parentCategoryService = new ParentCategoryService(context);
            string name = "Computers and Tablets";

            ParentCategory parentCategory = new ParentCategory
            {
                Name = name
            };
            context.ParentCategories.Add(parentCategory);
            context.SaveChanges();

            //Act
            ParentCategory category = this.parentCategoryService.CreateCategory(name);

            //Assert
            Assert.Null(category);
            Assert.Equal(1,context.ParentCategories.Count());
        }

        [Fact]
        public void GetParentCategories_ShouldReturnCollection()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            this.parentCategoryService = new ParentCategoryService(context);

            var categories = new List<ParentCategory>()
            {
                new ParentCategory { Name = "Computer and Tablets",SubCategories = new List<SubCategory>()
                {
                    new SubCategory { Name = "Desktop"},
                }},
                new ParentCategory { Name = "Components and Network", SubCategories = new List<SubCategory>()
                {
                    new SubCategory { Name = "CPU"},
                }},
                new ParentCategory { Name = "Electronic and Photo",SubCategories = new List<SubCategory>()
                {
                    new SubCategory { Name = "GPS"},
                }}
            };

            context.ParentCategories.AddRange(categories);
            context.SaveChanges();

            //Act
            var categoriesDb = this.parentCategoryService.GetParentCategories();

            //Assert
            Assert.NotEmpty(categoriesDb);
            Assert.Equal(categories.Count,categoriesDb.Count());
        }

        [Fact]
        public void GetParentCategories_WithNoCategory_ShouldReturnEmptyCollection()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            this.parentCategoryService = new ParentCategoryService(context);

            //Act
            var category = this.parentCategoryService.GetParentCategories();

            //Assert
            Assert.Empty(category);
        }

        [Fact]
        public void GetParentCategoryById_ShouldReturnCategory()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            this.parentCategoryService = new ParentCategoryService(context);
            ParentCategory parentCategory = new ParentCategory
            {
                Name = "Computers and Tablets"
            };
            context.ParentCategories.Add(parentCategory);
            context.SaveChanges();

            //Act
            ParentCategory category = this.parentCategoryService.GetParentCategoryById(parentCategory.Id);

            //Assert
            Assert.NotNull(category);
            Assert.Equal(parentCategory.Name, category.Name);
        }

        [Fact]
        public void GetParentCategoryById_WithInvalidId_ShouldReturnNull()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();
            this.parentCategoryService = new ParentCategoryService(context);

            //Act
            ParentCategory category = this.parentCategoryService.GetParentCategoryById(3);

            //Assert
            Assert.Null(category);
        }

        [Fact]
        public void DeleteCategory_ShouldReturnTrue()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            this.parentCategoryService = new ParentCategoryService(context);
            ParentCategory parentCategory = new ParentCategory
            {
                Name = "Computers and Tablets"
            };
            context.ParentCategories.Add(parentCategory);
            context.SaveChanges();

            //Act
            bool isDelete = this.parentCategoryService.DeleteCategory(parentCategory.Id);
            
            //Assert
            Assert.True(isDelete);
            Assert.Empty(context.ParentCategories);
        }

        [Fact]
        public void DeleteCategory_WithInvalidId_ShouldReturnFalse()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();
            this.parentCategoryService = new ParentCategoryService(context);

            //Act
            bool isDelete = this.parentCategoryService.DeleteCategory(3);

            //Assert
            Assert.False(isDelete);
        }

        [Fact]
        public void EditCategory_ShouldReturnTrue()
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();

            this.parentCategoryService = new ParentCategoryService(context);
            ParentCategory parentCategory = new ParentCategory
            {
                Name = "Computers and Tablets"
            };
            context.ParentCategories.Add(parentCategory);
            context.SaveChanges();

            string newName = "Computers";

            //Act
            bool isEdit = this.parentCategoryService.EditCategory(parentCategory.Id,newName);
            var category = context.ParentCategories.Single();

            //Assert
            Assert.True(isEdit);
            Assert.Equal(newName,category.Name);
        }

        [Theory]
        [InlineData(2,"New Name")]
        [InlineData(2,"    ")]
        public void DeleteCategory_WithInvalidParameters_ShouldReturnFalse(int id,string name)
        {
            //Arrange
            var context = OnlineShopDbContextInMemoryFactory.InitializeContext();
            this.parentCategoryService = new ParentCategoryService(context);

            //Act
            bool isEdit = this.parentCategoryService.EditCategory(id, name);
         

            //Assert
            Assert.False(isEdit);
        }
    }
}
