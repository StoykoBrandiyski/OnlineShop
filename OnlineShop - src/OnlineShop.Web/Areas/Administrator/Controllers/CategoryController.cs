using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Service.Data.Contracts;
using OnlineShop.Web.ViewModels.Administrator.Category;

namespace OnlineShop.Web.Areas.Administrator.Controllers
{
    public class CategoryController : AdministratorController
    {
        private readonly IParentCategoryService parentCategory;
        private readonly IMapper mapper;

        public CategoryController(IParentCategoryService parentCategory,IMapper mapper)
        {
            this.parentCategory = parentCategory;
            this.mapper = mapper;
        }


        public IActionResult All()
        {
            var category = this.parentCategory.GetParentCategories();

            var categoryViewModel = this.mapper.Map<IList<ParentCategoryViewModel>>(category);

            return View(categoryViewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ParentCategoryViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            this.parentCategory.CreateCategory(viewModel.Name);

            return RedirectToAction("All");
        }

        public IActionResult Edit(int id)
        {
            var category = this.parentCategory.GetParentCategoryById(id);

            if (category == null)
            {
                return RedirectToAction("All");
            }

            var categoryViewModel = this.mapper.Map<ParentCategoryViewModel>(category);

            return View(categoryViewModel);
        }

        [HttpPost]
        public IActionResult Edit(ParentCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool isEdit = this.parentCategory.EditCategory(model.Id,model.Name);

            return RedirectToAction("All");
        }

        public IActionResult Delete(int id)
        {
            bool isDelete = this.parentCategory.DeleteCategory(id);

            return RedirectToAction("All");
        }
    }
}