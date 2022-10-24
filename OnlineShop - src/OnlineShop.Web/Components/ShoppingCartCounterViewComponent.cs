using Microsoft.AspNetCore.Mvc;
using OnlineShop.Service.Data.Contracts;
using OnlineShop.Web.ViewModels.Componets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Web.Components
{
    [ViewComponent(Name = "ShoppingCartCounter")]
    public class ShoppingCartCounterViewComponent : ViewComponent
    {
        private IShoppingCartsService shoppingCartService;

        public ShoppingCartCounterViewComponent(IShoppingCartsService shoppingCartService)
        {
            this.shoppingCartService = shoppingCartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string username = this.User.Identity.Name;
            int counter = 0;

            if (username != null)
            {
                counter = this.shoppingCartService.GetAllShoppingCartProducts(username).Count();
            }

            var viewModel = new ShoppingCartCounterViewModel
            {
                CountProducts = counter
            };

            return this.View(viewModel);
        }
    }
}
