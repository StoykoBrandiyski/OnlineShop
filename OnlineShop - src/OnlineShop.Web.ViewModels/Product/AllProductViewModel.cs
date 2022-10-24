using System;
using System.Collections.Generic;
using System.Text;
using X.PagedList;

namespace OnlineShop.Web.ViewModels.Product
{
    public class AllProductViewModel
    {
        public int Id { get; set; }
        
        public IPagedList<ProductViewModel> Products { get; set; }

        public string SubCategory { get; set; }
        
        public int? PageSize { get; set; }

        public int? PageNumber { get; set; }

        public string KeyPartial { get; set; }
    }
}
