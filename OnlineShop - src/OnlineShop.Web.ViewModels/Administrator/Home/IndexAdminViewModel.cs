using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShop.Web.ViewModels.Administrator.Home
{
    public class IndexAdminViewModel
    {
        public IList<IndexProcessedOrdersViewModel> ProcessedOrders { get; set; }

        public IList<IndexUnprocessedOrdersViewModel> UnprocessedOrders { get; set; }


    }

}
