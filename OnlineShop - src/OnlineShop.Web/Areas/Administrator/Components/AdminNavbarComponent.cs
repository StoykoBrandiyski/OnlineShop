using Microsoft.AspNetCore.Mvc;
using OnlineShop.Service.Data.Contracts;
using OnlineShop.Web.ViewModels.Administrator.Home;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Web.Areas.Administrator.Components
{
    public class AdminNavbarComponent : ViewComponent
    {
        private readonly IOrderService orderService;

        public AdminNavbarComponent(IOrderService orderService)
        {
            this.orderService = orderService;
        }
        
        public IViewComponentResult Invoke()
        {
            int orderCount = this.orderService.GetUnprocessedOrders().Count();

            AdminNavbarViewModel viewModel = new AdminNavbarViewModel
            {
                UnprocessOrdersCount = orderCount
            };

            return View(viewModel);
        }
    }
}
