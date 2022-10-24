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
    public class SubCategoryServiceTests
    {
        private ISubCategoryService subCategoryService;
        private OnlineShopDbContext context;

        public SubCategoryServiceTests()
        {
            this.context = OnlineShopDbContextInMemoryFactory.InitializeContext();
        }
        
        [Fact]
        public async Task CreateSubCategory_ShouldOk()
        {
            //Arrange

            this.subCategoryService = new SubCategoryService(context);

            string expectedName = "smartphone description";
            string expectedDescription = "smartphone";
            string expectedKeyPartial = "smartphone";

            ParentCategory parentCategory = new ParentCategory
            {
                Name = "Mobile device"
            };

            await context.ParentCategories.AddAsync(parentCategory);
            await context.SaveChangesAsync();

            //Act
            SubCategory category = this.subCategoryService.CreateSubCategory(expectedName, expectedDescription,expectedKeyPartial, parentCategory.Id);

            //Assert
            Assert.NotNull(category);
            Assert.Equal(1, context.SubCategories.Count());
            Assert.Equal(expectedName, category.Name);
            Assert.Equal(expectedDescription, category.Description);
            Assert.Equal(parentCategory.Name, category.ParentCategory.Name);
        }

        [Theory]
        [InlineData(" ", "      "," ", 1)]
        [InlineData("     ", "","", 1)]
        public void CreateSubCategory_WithInvalidParameters_ShouldReturnNull(string name, string description,string keyPartial, int parentId)
        {
            //Arrange
            this.subCategoryService = new SubCategoryService(context);

            //Act
            SubCategory category = this.subCategoryService.CreateSubCategory(name, description,keyPartial, parentId);

            //Assert
            Assert.Null(category);
        }

        [Fact]
        public void CreateSubCategory_WithParentIdNotExist_ShouldReturnNull()
        {
            //Arrange
            string expectedName = "smartphone description";
            string expectedDescription = "smartphone";
            string expectedKeyPartial = "smartphone";

            this.subCategoryService = new SubCategoryService(context);

            //Act
            SubCategory category = this.subCategoryService.CreateSubCategory(expectedName, expectedDescription,expectedKeyPartial, 1);

            //Assert
            Assert.Null(category);
        }

        [Fact]
        public async Task GetSubCategoryById_ShouldOk()
        {
            //Arrange
            this.subCategoryService = new SubCategoryService(context);

            SubCategory category = new SubCategory
            {
                Name = "Smartphone",
                Description = "Smartphone description"
            };

            await context.SubCategories.AddAsync(category);
            await context.SaveChangesAsync();

            int categoryId = context.SubCategories.First().Id;

            //Act
            SubCategory categoryDb = this.subCategoryService.GetSubCategoryById(categoryId);

            //Assert
            Assert.NotNull(categoryDb);
            Assert.Equal(category.Name, categoryDb.Name);
            Assert.Equal(category.Description, categoryDb.Description);
        }

        [Fact]
        public void GetSubCategoryById_WithInvalidCategoryId_ShouldReturnNull()
        {
            //Arrange
            this.subCategoryService = new SubCategoryService(context);

            //Act
            SubCategory categoryDb = this.subCategoryService.GetSubCategoryById(3);

            //Assert
            Assert.Null(categoryDb);
        }

        [Fact]
        public async Task GetSubCategories_ShouldReturnCollection()
        {
            //Arrange
            this.subCategoryService = new SubCategoryService(context);

            var parentCategory = new ParentCategory { Name = "Computers" };

            await context.SubCategories.AddRangeAsync(new List<SubCategory>
            {
                new SubCategory { Name = "Cables", ParentCategoryId = parentCategory.Id, ParentCategory = parentCategory },
                new SubCategory { Name = "Monitors", ParentCategoryId = parentCategory.Id, ParentCategory = parentCategory }
            });
            await context.SaveChangesAsync();

            var childCategoriesService = new SubCategoryService(context);
            var childCategories = childCategoriesService.GetSubCategories();

            Assert.Equal(2, childCategories.Count());
        }

        [Fact]
        public void GetSubCategories_WithNoCategory_ShoulReturnNoCollection()
        {
            //Arrange
            this.subCategoryService = new SubCategoryService(context);

            //Act
            var categoriesDb = this.subCategoryService.GetSubCategories();

            //Assert
            Assert.Empty(categoriesDb);
        }

        [Fact]
        public async Task AddImageUrl_ShouldReturnTrue()
        {
            //Arrange
            this.subCategoryService = new SubCategoryService(context);

            SubCategory categories = new SubCategory
            {
                Name = "Smartphone",
                Description = "Smartphone description"
            };

            await context.SubCategories.AddAsync(categories);
            await context.SaveChangesAsync();

            var category = context.SubCategories.Single();

            //Act
            bool isAddedImage = this.subCategoryService.AddImageUrl(category.Id);


            //Assert
            Assert.True(isAddedImage);
            Assert.False(string.IsNullOrEmpty(category.ImageUrl));
        }

        [Fact]
        public void AddImageUrl_WithInvalidId_ShouldReturnFalse()
        {
            //Arrange
            this.subCategoryService = new SubCategoryService(context);

            //Act
            bool isAddedImage = this.subCategoryService.AddImageUrl(1);

            //Assert
            Assert.False(isAddedImage);
        }

        [Fact]
        public async Task DeleteSubCategory_ShouldReturnTrue()
        {
            //Arrange
            this.subCategoryService = new SubCategoryService(context);

            SubCategory categories = new SubCategory
            {
                Name = "Smartphone",
                Description = "Smartphone description"
            };

            await context.SubCategories.AddAsync(categories);
            await context.SaveChangesAsync();

            var category = context.SubCategories.Single();

            //Act
            bool isDelete = this.subCategoryService.DeleteSubCategory(category.Id);

            //Assert
            Assert.True(isDelete);
            Assert.Empty(context.SubCategories);
        }

        [Fact]
        public void DeleteSubCategory_WithInvalidCategoryId_ShouldReturnFalse()
        {
            //Arrange
            this.subCategoryService = new SubCategoryService(context);

            //Act
            bool isDelete = this.subCategoryService.AddImageUrl(1);

            //Assert
            Assert.False(isDelete);
        }

        [Fact]
        public async Task EditSubCategory_ShouldReturnTrue()
        {
            //Arrange
            this.subCategoryService = new SubCategoryService(context);

            string newName = "Smart";
            string newDescription = "New Description";


            SubCategory categories = new SubCategory
            {
                Name = "Smartphone",
                Description = "Smartphone description",
                KeyPartial = "smartphone",
                ParentCategory = new ParentCategory
                {
                    Name = "Mobile devices"
                }
            };

            await context.SubCategories.AddAsync(categories);
            await context.SaveChangesAsync();

            //Act
            bool isEdit = this.subCategoryService.EditSubCategory(categories.Id, newName, newDescription,categories.KeyPartial, categories.ParentCategory.Id);

            var category = context.SubCategories.Single();

            //Assert
            Assert.True(isEdit);
            Assert.Equal(newName, category.Name);
            Assert.Equal(newDescription, category.Description);
        }

        [Fact]
        public void EditSubCategory_WithInvalidCategoryID_ShouldReturnFalse()
        {
            //Arrange
            this.subCategoryService = new SubCategoryService(context);

            //Act
            bool isEdit = this.subCategoryService.EditSubCategory(1, "Smart", "Smart Description","smartphone", 2);

            //Assert
            Assert.False(isEdit);
        }

        [Fact]
        public async Task EditSubCategory_WithInvalidParameters_ShouldReturnFalse()
        {
            //Arrange
            this.subCategoryService = new SubCategoryService(context);

            SubCategory category = new SubCategory
            {
                Name = "Smartphone",
                Description = "Smartphone description",
                ParentCategory = new ParentCategory
                {
                    Name = "Mobile devices"
                }
            };

            await context.SubCategories.AddAsync(category);
            await context.SaveChangesAsync();

            //Act
            bool isEdit = this.subCategoryService.EditSubCategory(category.Id, "", "            ","", 1);

            var categoryDb = context.SubCategories.Single();

            //Assert
            Assert.False(isEdit);
            Assert.Equal(category.Name, categoryDb.Name);
            Assert.Equal(category.Description, categoryDb.Description);
            Assert.Equal(category.ParentCategory.Name, categoryDb.ParentCategory.Name);
        }

        [Fact]
        public async Task EditSubCategory_WithInvalidParentCategoryId_ShouldReturnFalse()
        {
            //Arrange
            this.subCategoryService = new SubCategoryService(context);

            SubCategory categories = new SubCategory
            {
                Name = "Smartphone",
                Description = "Smartphone description",
            };

            await context.SubCategories.AddAsync(categories);
            await context.SaveChangesAsync();

            //Act
            bool isEdit = this.subCategoryService.EditSubCategory(categories.Id, "Smart", "Smart Description","smartphone", 2);


            //Assert
            Assert.False(isEdit);
        }
    }
}