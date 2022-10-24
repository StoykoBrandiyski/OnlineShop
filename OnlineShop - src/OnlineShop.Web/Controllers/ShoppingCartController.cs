using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models.Enums;
using OnlineShop.Service.Data.Contracts;
using OnlineShop.Web.ViewModels.ShoppingCart;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Web.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartsService shoppingCarts;
        private readonly IMapper mapper;
        private readonly IProductService productService;

        public ShoppingCartController(IShoppingCartsService shoppingCarts, IMapper mapper,IProductService productService)
        {
            this.shoppingCarts = shoppingCarts;
            this.mapper = mapper;
            this.productService = productService;
        }

        public IActionResult Index()
        {
            var shoppingCardProducts = this.shoppingCarts.GetAllShoppingCartProducts(User.Identity.Name);

            bool isPartnerOrAdmin = this.User.IsInRole(UserRole.Admin.ToString()) || this.User.IsInRole(UserRole.Partner.ToString());

            var shoppingCartProductsViewModel = shoppingCardProducts.Select(x => new ShoppingCartProductsViewModel
            {
                Id = x.ProductId,
                ImageUrl = x.Product.Images.FirstOrDefault()?.ImageUrl,
                Name = x.Product.Name,
                Price = isPartnerOrAdmin ? x.Product.ParnersPrice : x.Product.Price,
                Quantity = x.Quantity,
                TotalPrice = x.Quantity * (isPartnerOrAdmin ? x.Product.ParnersPrice : x.Product.Price)
            }).ToList();

            return View(shoppingCartProductsViewModel);
        }

        public async Task<IActionResult> Add(string id)
        {
            var product = this.productService.GetProductById(id);

            if (product == null)
            {
                return RedirectToAction("Product", "Details", id);
            }

            await this.shoppingCarts.AddProductInShoppingCart(id, User.Identity.Name);
            
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string id)
        {
            var product = this.productService.GetProductById(id);

            if (product == null)
            {
                return RedirectToAction("Index");
            }

            await this.shoppingCarts.DeleteProductFromShoppingCart(id, User.Identity.Name);

            return RedirectToAction(nameof(Index));
        }

    }
}