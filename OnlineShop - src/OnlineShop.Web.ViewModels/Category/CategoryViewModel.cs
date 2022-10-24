using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShop.Web.ViewModels.Category
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        public IList<SubCategoryViewModel> SubCategories { get; set; }

    }
}
