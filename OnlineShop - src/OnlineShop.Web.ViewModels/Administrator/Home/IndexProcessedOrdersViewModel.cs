using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShop.Web.ViewModels.Administrator.Home
{
    public class IndexProcessedOrdersViewModel
    {
        public int Id { get; set; }

        public string PaymentStatus { get; set; }

        public DateTime? DispatchDate { get; set; }

        public decimal TotalPrice { get; set; }

        public string PaymentType { get; set; }
    }
}
