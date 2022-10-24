using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Service.Data.Contracts;
using OnlineShop.Web.ViewModels.Administrator.Home;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShop.Web.Areas.Administrator.Controllers
{
    public class HomeController : AdministratorController
    {
        private readonly IOrderService orderService;
        private readonly IMapper mapper;

        public HomeController(IOrderService orderService,IMapper mapper)
        {
            this.orderService = orderService;
            this.mapper = mapper;
        }


        public IActionResult Index()
        {
            var unprocessedOrders = this.orderService.GetUnprocessedOrders()
                .OrderByDescending(order => order.OrderDate);

            var processedOrders = this.orderService.GetProcessedOrders()
                .OrderByDescending(order => order.DispatchDate);
            
            var unprocessedOrdersViewModel = this.mapper.Map<IList<IndexUnprocessedOrdersViewModel>>(unprocessedOrders);
            var processedOrdersViewModel = this.mapper.Map<IList<IndexProcessedOrdersViewModel>>(processedOrders);

            var model = new IndexAdminViewModel
            {
                ProcessedOrders = processedOrdersViewModel,
                UnprocessedOrders = unprocessedOrdersViewModel
            };

            return View(model);
        }
    }
}