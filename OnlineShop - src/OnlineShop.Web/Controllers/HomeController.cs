using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Service.Data.Contracts;
using OnlineShop.Web.Models;
using OnlineShop.Web.ViewModels.Home;

namespace OnlineShop.Web.Controllers
{
    public class HomeController : Controller
    {
        private const string NO_RESULTS_FOUND = "Няма намерени резултати";

        private readonly IProductService productService;
        private readonly IMapper mapper;

        public HomeController(IProductService productService, IMapper mapper)
        {
            this.productService = productService;
            this.mapper = mapper;
        }


        public async Task<IActionResult> Index(IndexViewModel model)
        {
            var products = string.IsNullOrEmpty(model.SearchString) 
                               ? await this.productService.GetAllProductByCategoryId(1)
                               : this.productService.GetProductsBySearch(model.SearchString);

            if (products.Count() == 0)
            {
                model.Products = new List<IndexProductViewModel>();
                model.CountAllProduct = 0;
                return View(model);
            }

            //string url = products.FirstOrDefault().Images.FirstOrDefault().ImageUrl;

            IList<IndexProductViewModel> productsViewModel = this.mapper.Map<IList<IndexProductViewModel>>(products);

            model.Products = productsViewModel;
            model.CountAllProduct = products.Count();

            return View(model);
        }

        public IActionResult GetProduct(string term)
        {
            var products = this.productService.GetProductsBySearch(term);

            if (products.Count() == 0)
            {
                return Json(new List<SearchViewModel>
                {
                    new SearchViewModel(){Value = NO_RESULTS_FOUND, Url = string.Empty}
                });
            }

            var result = products.Select(x => new SearchViewModel
            {
                Value = x.Name,
                Url = string.Format(GlobalConstants.URL_TEMPLATE_AUTOCOMPLETE, x.Id)
            });

            return Json(result);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
