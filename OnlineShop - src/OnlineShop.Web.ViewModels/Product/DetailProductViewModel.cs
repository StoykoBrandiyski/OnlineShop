using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShop.Web.ViewModels.Product
{
    public class DetailProductViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }
        
        public string Description { get; set; }

        public Dictionary<string,string> SpecificationParameters { get; set; }

        public string Price { get; set; }

        public string ParnersPrice { get; set; }

        public ICollection<string> Images { get; set; }
    }
}
