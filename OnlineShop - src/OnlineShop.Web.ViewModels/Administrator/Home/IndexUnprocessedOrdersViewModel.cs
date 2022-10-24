using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShop.Web.ViewModels.Administrator.Home
{
    public class IndexUnprocessedOrdersViewModel
    {
        public int Id { get; set; }

        public string PaymentStatus { get; set; }

        public DateTime? OrderDate { get; set; }

        public decimal TotalPrice { get; set; }

        public string PaymentType { get; set; }
    }
}
