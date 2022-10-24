using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models.Enums;
using OnlineShop.Service.Data.Contracts;
using OnlineShop.Web.ViewModels.Order;
using OnlineShop.Web.ViewModels.ShoppingCart;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Web.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private const string ERROR_MESSAGE_TO_CONTINUE_ADD_PRODUCTS = "За да продължите добавете продукти в кошницата!";
        private const string YOUR_ORDER_WAS_SUCCESSFULLY_RECEIVED = "Вашата поръчка беше получена успешно!";

        private readonly IProductService productService;
        private readonly IShoppingCartsService shoppingCartsService;
        private readonly IOrderService orderService;
        private readonly IMapper mapper;
        private readonly IAddresService addresService;
        private readonly IUserService userService;

        public OrderController(IProductService productService, IShoppingCartsService shoppingCartsService, 
            IOrderService orderService, IMapper mapper,IAddresService addresService,
            IUserService userService)
        {
            this.productService = productService;
            this.shoppingCartsService = shoppingCartsService;
            this.orderService = orderService;
            this.mapper = mapper;
            this.addresService = addresService;
            this.userService = userService;
        }


        public IActionResult Create()
        {
            var user = this.userService.GetUserByUsername(this.User.Identity.Name);
            bool isPartnerOrAdmin = this.User.IsInRole(UserRole.Admin.ToString()) || this.User.IsInRole(UserRole.Partner.ToString());

            var shoppingCartProducts = this.shoppingCartsService.GetAllShoppingCartProducts(user.UserName);

            if (shoppingCartProducts == null)
            {
                this.TempData["error"] = ERROR_MESSAGE_TO_CONTINUE_ADD_PRODUCTS;
                return RedirectToAction("Index", "Home");
            }

            var addresses = this.addresService.GetAllUserAddress(user.UserName);
            var addressesViewModel = this.mapper.Map<IList<OrderAdressViewModel>>(addresses);

            var shoppingCartProductsViewModel = shoppingCartProducts.Select(x => new ShoppingCartProductsViewModel
            {
                Id = x.ProductId,
                ImageUrl = x.Product.Images.FirstOrDefault()?.ImageUrl,
                Name = x.Product.Name,
                Price = isPartnerOrAdmin ? x.Product.ParnersPrice : x.Product.Price,
                Quantity = x.Quantity,
                TotalPrice = x.Quantity * (isPartnerOrAdmin ? x.Product.ParnersPrice : x.Product.Price)
            }).ToList();

            string fullName = $"{user.FirstName} {user.LastName}";
            
            CreateOrderViewModel viewModel = new CreateOrderViewModel
            {
                OrderAddressesViewModel = addressesViewModel.ToList(),
                FullName = fullName,
                PhoneNumber = user.PhoneNumber,
                Products = shoppingCartProductsViewModel
            };

            return View(viewModel);
        }


        [HttpPost]
        public IActionResult Create(CreateOrderViewModel model)
        {
            string username = this.User.Identity.Name;

            if (!this.shoppingCartsService.AnyProducts(username))
            {
                this.TempData["error"] = ERROR_MESSAGE_TO_CONTINUE_ADD_PRODUCTS;
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Create));
            }

            var order = this.orderService.GetProcessingOrder(username);
            if (order == null)
            {
                order = this.orderService.CreateOrder(username);
            }

            decimal deliveryPrice = 6.80M;
            this.orderService.SetOrderDetails(order, model.FullName, model.PhoneNumber,
                PaymentType.CashОnDelivery,model.DeliveryAddressId.Value, deliveryPrice);

            return RedirectToAction(nameof(Complete));
        }

        public IActionResult Complete()
        {
            string username = this.User.Identity.Name;

            if (!this.shoppingCartsService.AnyProducts(username))
            {
                this.TempData["error"] = ERROR_MESSAGE_TO_CONTINUE_ADD_PRODUCTS;
                return RedirectToAction("Index", "Home");
            }

            var order = this.orderService.GetProcessingOrder(username);
            var orderViewModel = mapper.Map<ConfirmOrderViewModel>(order);

            bool isPartnerOrAdmin = this.User.IsInRole(UserRole.Admin.ToString()) || this.User.IsInRole(UserRole.Partner.ToString());
            this.orderService.CompleteProcessingOrder(this.User.Identity.Name, isPartnerOrAdmin);
            
            this.TempData["info"] = YOUR_ORDER_WAS_SUCCESSFULLY_RECEIVED;
            
            return this.View(orderViewModel);
        }
    }
}