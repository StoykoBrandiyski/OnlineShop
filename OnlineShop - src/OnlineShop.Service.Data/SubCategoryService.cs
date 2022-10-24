using Common;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;
using OnlineShop.Service.Data.Contracts;
using OnlineShop.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShop.Service.Data
{
    public class SubCategoryService : ISubCategoryService
    {
        private readonly OnlineShopDbContext dbContext;

        public SubCategoryService(OnlineShopDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        public bool AddImageUrl(int id)
        {
            SubCategory categoryDb = this.dbContext.SubCategories
                .FirstOrDefault(category => category.Id == id);

            if (categoryDb == null)
            {
                return false;
            }

            categoryDb.ImageUrl = string.Format(GlobalConstants.SUB_CATEGORY_SRC_ROOT_TEMPLATE, id);
            this.dbContext.SaveChanges();

            return true;
        }
        
        public SubCategory CreateSubCategory(string name, string description,string keyPartial, int parentId)
        {
            if (string.IsNullOrWhiteSpace(name) 
                || string.IsNullOrWhiteSpace(description) 
                || parentId <= 0 )
            {
                return null;
            }

            ParentCategory parentCategory = this.dbContext.ParentCategories
                .FirstOrDefault(category => category.Id == parentId);

            if (parentCategory == null)
            {
                return null;
            }

            SubCategory newCategory = new SubCategory
            {
                Name = name,
                Description = description,
                KeyPartial = keyPartial,
                ParentCategoryId = parentId
            };

            this.dbContext.SubCategories.Add(newCategory);
            this.dbContext.SaveChanges();

            return newCategory;
        }

        public bool DeleteSubCategory(int id)
        {
            SubCategory categoryDb = this.dbContext.SubCategories
                .Include(category => category.Products)
                .FirstOrDefault(category => category.Id == id);

            if(categoryDb == null || categoryDb.Products.Any())
            {
                return false;
            }

            this.dbContext.SubCategories.Remove(categoryDb);
            this.dbContext.SaveChanges();

            return true;
        }

        public bool EditSubCategory(int id, string name, string description,string keyPartial, int parentId)
        {
            SubCategory categoryDb = this.dbContext.SubCategories
               .FirstOrDefault(category => category.Id == id);

            bool parentCategoryIsExist = this.dbContext.ParentCategories.Any(category => category.Id == parentId);

            if (categoryDb == null || string.IsNullOrWhiteSpace(name)
                || string.IsNullOrWhiteSpace(description) || !parentCategoryIsExist)
            {
                return false;
            }

            categoryDb.Name = name;
            categoryDb.Description = description;
            categoryDb.KeyPartial = keyPartial;
            categoryDb.ParentCategoryId = parentId;

            this.dbContext.SaveChanges();

            return true;
        }

        public IEnumerable<SubCategory> GetSubCategories()
        {
            var categories = this.dbContext.SubCategories
                                    .Include(category => category.Products)
                                    .Include(category => category.ParentCategory)
                                    .ToList();
            return categories;
        }

        public IEnumerable<SubCategory> GetSubCategoriesByParentId(int id)
        {
            var categories = this.dbContext.SubCategories
                .Where(x => x.ParentCategoryId == id)
                .ToList();

            return categories;
        }

        public SubCategory GetSubCategoryById(int id)
        {
            SubCategory categoryDb = this.dbContext.SubCategories.FirstOrDefault(category => category.Id == id);

            return categoryDb;
        }
    }
}
