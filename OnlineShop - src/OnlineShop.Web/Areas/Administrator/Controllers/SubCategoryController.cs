using AutoMapper;
using Common;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Service.Data.Contracts;
using OnlineShop.Web.ViewModels.Administrator.Category;
using OnlineShop.Web.ViewModels.Administrator.SubCategory;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShop.Web.Areas.Administrator.Controllers
{
    public class SubCategoryController : AdministratorController
    {

        private readonly ISubCategoryService subCategory;
        private readonly IMapper mapper;
        private readonly IParentCategoryService parentCategory;
        private readonly IImageService imageService;

        public SubCategoryController(ISubCategoryService subCategory, IMapper mapper,
            IParentCategoryService parentCategory,IImageService imageService)
        {
            this.subCategory = subCategory;
            this.mapper = mapper;
            this.parentCategory = parentCategory;
            this.imageService = imageService;
        }


        public IActionResult All()
        {
            var subCategory = this.subCategory.GetSubCategories();
            var subCategoryViewModel = this.mapper.Map<IList<SubCategoryViewModel>>(subCategory);

            var parentCategories = this.parentCategory.GetParentCategories();
            var parentCategoriesViewModel = this.mapper.Map<IList<ParentCategoryViewModel>>(parentCategories);
           
            var viewModel = new AllSubParentCategoryViewModel
            {
                ParentCategories = parentCategoriesViewModel,
                SubCategories = subCategoryViewModel
            };

            return View(viewModel);
        }

        public IActionResult Create()
        {
            var parentCategory = this.parentCategory.GetParentCategories().ToList();
            var parentCategoryViewModel = this.mapper.Map<IList<ParentCategoryViewModel>>(parentCategory);

            var viewModel = new CreateSubCategoryViewModel
            {
                ParentCategories = parentCategoryViewModel
            };

            return View(viewModel);
        }
        
        [HttpPost]
        public IActionResult Create(CreateSubCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var parentCategory = this.parentCategory.GetParentCategories().ToList();
                var parentCategoryViewModel = this.mapper.Map<IList<ParentCategoryViewModel>>(parentCategory);

                model.ParentCategories = parentCategoryViewModel;
                return View(model);
            }

            var createdSubCategory = this.subCategory.CreateSubCategory(model.Name, model.Description,model.KeyPartial, model.ParentId);

            if (model.FormImage != null)
            {
                string imageUrl = string.Format(GlobalConstants.SUB_CATEGORY_PATH_TEMPLATE, createdSubCategory.Id);

                this.imageService.UploadImage(model.FormImage, imageUrl);

                this.subCategory.AddImageUrl(createdSubCategory.Id);
            }

            return RedirectToAction("All");
        }
        
        public IActionResult Edit(int id)
        {
            var subCategory = this.subCategory.GetSubCategoryById(id);
            var parentCategory = this.parentCategory.GetParentCategories();

            if (subCategory == null)
            {
                return RedirectToAction("All");
            }

            var subCategoryViewModel = this.mapper.Map<EditSubCategoryViewModel>(subCategory);
            subCategoryViewModel.ParentCategories = this.mapper.Map<IList<ParentCategoryViewModel>>(parentCategory);

            return View(subCategoryViewModel);
        }

        [HttpPost]
        public IActionResult Edit(EditSubCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("All");
            }

            bool isEdit = this.subCategory.EditSubCategory(model.Id, model.Name, model.Description,
                                                            model.KeyPartial,model.ParentId);
            
            if (model.FormImage != null)
            {
                string imageUrl = string.Format(GlobalConstants.SUB_CATEGORY_PATH_TEMPLATE, model.Id);

                this.imageService.UploadImage(model.FormImage, imageUrl);

                this.subCategory.AddImageUrl(model.Id);
            }
            
            return RedirectToAction("All");
        }


        public IActionResult Delete(int id)
        {
            bool isDelete = this.subCategory.DeleteSubCategory(id);

            if (!isDelete)
            {
                this.TempData["error"] = GlobalConstants.CANNOT_DELETE_CATEGORY_IF_ANY_PRODUCTS;
            }

            return RedirectToAction("All");
        }

    }
}