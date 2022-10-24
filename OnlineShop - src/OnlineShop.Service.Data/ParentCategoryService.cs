using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;
using OnlineShop.Service.Data.Contracts;
using OnlineShop.Web.Data;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShop.Service.Data
{
    public class ParentCategoryService : IParentCategoryService
    {
        private readonly OnlineShopDbContext dbContext;

        public ParentCategoryService(OnlineShopDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        public ParentCategory CreateCategory(string name)
        {
            ParentCategory parentCategory = this.dbContext.ParentCategories
                    .FirstOrDefault(category => category.Name == name);

            if (parentCategory != null)
            {
                return null;
            }

            parentCategory = new ParentCategory
            {
                Name = name
            };

            this.dbContext.ParentCategories.Add(parentCategory);
            this.dbContext.SaveChanges();

            return parentCategory;
        }

        public bool DeleteCategory(int id)
        {
            ParentCategory parentCategory = this.dbContext.ParentCategories
                   .FirstOrDefault(category => category.Id == id);

            if (parentCategory == null)
            {
                return false;
            }
            
            this.dbContext.ParentCategories.Remove(parentCategory);
            this.dbContext.SaveChanges();

            return true;
        }

        public bool EditCategory(int id,string name)
        {
            ParentCategory parentCategory = this.dbContext.ParentCategories
                  .FirstOrDefault(category => category.Id == id);

            if (parentCategory == null || string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            parentCategory.Name = name;
            this.dbContext.SaveChanges();

            return true;
        }

        public IEnumerable<ParentCategory> GetParentCategories()
        {
            var categories = this.dbContext.ParentCategories
                .Include(c => c.SubCategories)
                .ToArray();

            return categories;
        }

        public ParentCategory GetParentCategoryById(int id)
        {
            ParentCategory parentCategory = this.dbContext.ParentCategories
                   .FirstOrDefault(category => category.Id == id);
            
            return parentCategory;
        }
    }
}
