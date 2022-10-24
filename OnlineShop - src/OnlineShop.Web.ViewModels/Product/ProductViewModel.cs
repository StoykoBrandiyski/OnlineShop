using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShop.Web.ViewModels.Product
{
    public class ProductViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }
        
        public decimal Price { get; set; }

        public decimal ParnersPrice { get; set; }

        public string ImageUrl { get; set; }
    }
}
