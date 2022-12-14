using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Web.ViewModels.Home
{
    public class IndexProductViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }
        
        public decimal Price { get; set; }

        public decimal ParnersPrice { get; set; }
        
        public string ImageUrl { get; set; }
    }
}
