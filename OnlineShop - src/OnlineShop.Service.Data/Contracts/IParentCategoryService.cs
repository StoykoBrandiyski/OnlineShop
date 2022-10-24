using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShop.Service.Data.Contracts
{
    public interface IParentCategoryService
    {
        ParentCategory CreateCategory(string name);

        IEnumerable<ParentCategory> GetParentCategories();

        ParentCategory GetParentCategoryById(int id);

        bool DeleteCategory(int id);

        bool EditCategory(int id,string name);
    }
}
