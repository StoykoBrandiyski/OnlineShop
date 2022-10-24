using OnlineShop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShop.Service.Data.Contracts
{
    public interface ISubCategoryService
    {
        IEnumerable<SubCategory> GetSubCategories();

        SubCategory CreateSubCategory(string name, string description, string keyPartial, int parentId);

        SubCategory GetSubCategoryById(int id);

        bool AddImageUrl(int id);

        bool EditSubCategory(int id, string name, string description,string keyPartial, int parentId);

        bool DeleteSubCategory(int id);

        //Not Testing
        IEnumerable<SubCategory> GetSubCategoriesByParentId(int id);
    }
}
