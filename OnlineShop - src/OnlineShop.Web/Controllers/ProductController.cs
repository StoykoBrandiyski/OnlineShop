using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Service.Data.Contracts;
using System.Collections.Generic;
using OnlineShop.Web.ViewModels.Product;
using System.Threading.Tasks;
using System;
using System.Threading;
using System.Linq;
using X.PagedList;

namespace OnlineShop.Web.Controllers
{
    public class ProductController : Controller
    {
        private const int DEFAULT_PAGE_SIZE = 8;
        private const int DEFAULT_PAGE_NUMBER = 1;

        private readonly IProductService productService;
        private readonly IMapper mapper;
        private readonly ISubCategoryService subCategoryService;

        public ProductController(IProductService productService, IMapper mapper,
            ISubCategoryService subCategoryService)
        {
            this.productService = productService;
            this.mapper = mapper;
            this.subCategoryService = subCategoryService;
        }


        public async Task<IActionResult> All(AllProductViewModel model)
        {
            var products = await this.productService.GetAllProductByCategoryId(model.Id);
            var subCategory = this.subCategoryService.GetSubCategoryById(model.Id);
            
            model.PageNumber = model.PageNumber ?? DEFAULT_PAGE_NUMBER;
            model.PageSize = model.PageSize ?? DEFAULT_PAGE_SIZE;

            if (subCategory == null || products.Count() == 0)
            {
                model.Products = new List<ProductViewModel>()
                            .ToPagedList(model.PageNumber.Value, model.PageSize.Value);
                return View(model);
            }

            model.SubCategory = subCategory.Name;
            
            var productsViewModel = this.mapper.Map<IList<ProductViewModel>>(products);

            var pageProductViewModel = productsViewModel.ToPagedList(model.PageNumber.Value, model.PageSize.Value);
            
            model.Products = pageProductViewModel;
            model.KeyPartial = subCategory.KeyPartial;

            return View(model);
        }
        
        public async Task<IActionResult> Details(string id)
        {
            var product = await this.productService.GetProductById(id);

            if(product == null)
            {
                return NotFound();
            }

            DetailProductViewModel viewModel = this.mapper.Map<DetailProductViewModel>(product);

            //Take out
            string[] specification = product.Specification.Split(new []{ ';' },StringSplitOptions.RemoveEmptyEntries);
            
            viewModel.SpecificationParameters = new Dictionary<string, string>();
            foreach (var item in specification)
            {
                var keyValue = item.Split(':');

                viewModel.SpecificationParameters.Add(keyValue[0], keyValue[1]);
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> GetProductsDefault(int id)
        {
            var products = await this.productService.GetAllProductByCategoryId(id);

            if (id <= 0 || products.Count() == 0)
            {
                return Json(null);
            }
            
            var productsViewModel = this.mapper.Map<IList<ProductViewModel>>(products);
            
            return Json(productsViewModel);
        }
        
        [HttpPost]
        public async Task<IActionResult> GetFilterProductsByOrder(string typeOrder, int id)
        {
            //var category = this.subCategoryService.GetSubCategoryById(id);
            var products = await this.productService.GetAllProductByCategoryId(id);

            if (string.IsNullOrEmpty(typeOrder) || products.Count() == 0)
            {
                return Json(null);
            }

            products = this.productService.OrderBy(products, typeOrder);

            var productsViewModel = this.mapper.Map<IList<ProductViewModel>>(products);
            return Json(productsViewModel);
        }

        [HttpPost]
        public IActionResult GetFilter(string type,string value,int categoryId)
        {
            var category = this.subCategoryService.GetSubCategoryById(categoryId);

            if (string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(value)
                    || category == null)
            {
                return Json(null);
            }

            var products = this.productService.GetProductsByFilter(type, value,category);

            var productsViewModel = this.mapper.Map<IList<ProductViewModel>>(products);

            return Json(productsViewModel);
        }

        [HttpPost]
        public IActionResult GetFilterByPrice(string value, int categoryId)
        {
            var category = this.subCategoryService.GetSubCategoryById(categoryId);

            if (string.IsNullOrWhiteSpace(value) || category == null)
            {
                return Json(null);
            }

            //Take out
            string[] argm = value.Split('-');
            decimal minPrice = decimal.Parse(argm[0]);
            decimal maxPrice = decimal.Parse(argm[1]);
            
            var products = this.productService.GetProductsByFilterPrice(minPrice,maxPrice,category);

            var productsViewModel = this.mapper.Map<IList<ProductViewModel>>(products);

            return Json(productsViewModel);
        }

        [HttpPost]
        public IActionResult GetFilterByBrand(string value,int categoryId)
        {
            var category = this.subCategoryService.GetSubCategoryById(categoryId);

            if (string.IsNullOrWhiteSpace(value) || category == null)
            {
                return Json(null);
            }

            var products = this.productService.GetProductsByFilterBrand(value,category);

            var productsViewModel = this.mapper.Map<IList<ProductViewModel>>(products);

            return Json(productsViewModel);
        }

    }
}