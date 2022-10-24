using AutoMapper;
using Common;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using OnlineShop.Service.Data.Contracts;
using OnlineShop.Web.ViewModels.Administrator.Product;
using OnlineShop.Web.ViewModels.Administrator.SubCategory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace OnlineShop.Web.Areas.Administrator.Controllers
{
    public class ProductController : AdministratorController
    {
        private const int DEFAULT_PAGE_SIZE = 8;
        private const int DEFAULT_PAGE_NUMBER = 1;

        private readonly IProductService productService;
        private readonly IMapper mapper;
        private readonly ISubCategoryService subCategory;
        private readonly IImageService imageService;

        public ProductController(IProductService productService, IMapper mapper,
            ISubCategoryService subCategory,IImageService imageService)
        {
            this.productService = productService;
            this.mapper = mapper;
            this.subCategory = subCategory;
            this.imageService = imageService;
        }


        public IActionResult All(int? pageSize, int? pageNumber)
        {
            var products = this.productService.GetAllProducts();

            pageNumber = pageNumber ?? DEFAULT_PAGE_NUMBER;
            pageSize = pageSize ?? DEFAULT_PAGE_SIZE;

            var pageProductViewModel = products.ToPagedList(pageNumber.Value, pageSize.Value);

            return View(pageProductViewModel);
        }

        public IActionResult AllHide(int? pageSize, int? pageNumber)
        {
            var products = this.productService.GetHideProducts();

            pageNumber = pageNumber ?? DEFAULT_PAGE_NUMBER;
            pageSize = pageSize ?? DEFAULT_PAGE_SIZE;

            var pageProductViewModel = products.ToPagedList(pageNumber.Value, pageSize.Value);

            return View(pageProductViewModel);
        }

        public IActionResult Create()
        {
            var subCategory = this.subCategory.GetSubCategories();

            var subCategoryViewModel = this.mapper.Map<IList<SubCategoryViewModel>>(subCategory);

            var viewModel = new CreateProductViewModel
            {
                SubCategories = subCategoryViewModel
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var subCategory = this.subCategory.GetSubCategories();
                
                model.SubCategories = this.mapper.Map<IList<SubCategoryViewModel>>(subCategory);
                
                return View(model);
            }

            Product product = this.mapper.Map<Product>(model);

            await this.productService.AddProduct(product);

            if (model.FormImages != null)
            {
                int existingImages = 0;
                var imageUrls = await this.imageService.UploadImages(model.FormImages.ToList(), existingImages,
                                                GlobalConstants.PRODUCT_PATH_TEMPLATE, product.Id);

                this.productService.AddImageUrls(product.Id,imageUrls);
            }

            return RedirectToAction("All");
        }

        public async Task<IActionResult> Edit(string id)
        {
            var subCategories = this.subCategory.GetSubCategories();
            var productDb = await this.productService.GetProductById(id);

            var subCategoryViewModel = this.mapper.Map<IList<SubCategoryViewModel>>(subCategories);
            var productViewModel = this.mapper.Map<EditProductViewModel>(productDb);

            productViewModel.SubCategories = subCategoryViewModel;

            return View(productViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var subCategory = this.subCategory.GetSubCategories();

                var subCategoryViewModel = this.mapper.Map<IList<SubCategoryViewModel>>(subCategory);

                model.SubCategories = subCategoryViewModel;

                return View(model);
            }

            Product product = this.mapper.Map<Product>(model);

            bool isEdit = await this.productService.EditProduct(product);

            if (model.FormImages != null)
            {
                int existingImages = model.FormImages.Count;
                var imageUrls = await this.imageService.UploadImages(model.FormImages.ToList(), existingImages,
                                                GlobalConstants.PRODUCT_PATH_TEMPLATE, product.Id);

                this.productService.AddImageUrls(product.Id, imageUrls);
            }

            return RedirectToAction("All");
        }

        public IActionResult Hide(string id)
        {
            this.productService.HideProduct(id);

            return RedirectToAction("All");
        }

        public IActionResult Show(string id)
        {
            this.productService.ShowProduct(id);

            return RedirectToAction("All");
        }
    }
}