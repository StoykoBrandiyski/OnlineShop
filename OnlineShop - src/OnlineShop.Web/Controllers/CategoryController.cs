using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Service.Data.Contracts;
using OnlineShop.Web.ViewModels.Category;

namespace OnlineShop.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IParentCategoryService parentCategory;
        private readonly ISubCategoryService subCategory;
        private readonly IMapper mapper;

        public CategoryController(IParentCategoryService parentCategory,
                    ISubCategoryService subCategory,IMapper mapper)
        {
            this.parentCategory = parentCategory;
            this.subCategory = subCategory;
            this.mapper = mapper;
        }


        public IActionResult ParentCategory(CategoryViewModel model)
        {
            int parentCategoryId = model.Id;

            var categories = this.subCategory.GetSubCategoriesByParentId(parentCategoryId);

            var subCategoriesViewModel = this.mapper.Map<IList<SubCategoryViewModel>>(categories);

            model.SubCategories = subCategoriesViewModel;

            return View(model);
        }
    }
}