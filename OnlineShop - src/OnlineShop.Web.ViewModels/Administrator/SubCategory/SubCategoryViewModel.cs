using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShop.Web.ViewModels.Administrator.SubCategory
{
    public class SubCategoryViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ParentCategoryName { get; set; }

        public int ProductsCount { get; set; }
    }
}
