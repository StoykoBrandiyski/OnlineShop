using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineShop.Models;
using OnlineShop.Service.Data.Contracts;

namespace OnlineShop.Web.Areas.Identity.Pages.Account.Manage
{
    public class UserOrderModel : PageModel
    {
        private readonly IOrderService orderService;
        private readonly IUserService userService;

        public UserOrderModel(IOrderService orderService, IUserService userService)
        {
            this.orderService = orderService;
            this.userService = userService;
        }

        [BindProperty]
        public OrderModel Input { get; set; }

        public class OrderModel
        {
            public ICollection<Order> Orders { get; set; }
        }


        public IActionResult OnGet()
        {
            var orders = this.orderService.GetUserOrders(this.User.Identity.Name);

            Input = new OrderModel
            {
                Orders = orders.ToList()
            };

            return Page();
        }
    }
}