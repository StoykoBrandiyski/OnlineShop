using OnlineShop.Web.ViewModels.Administrator.Category;
using System.Collections.Generic;

namespace OnlineShop.Web.ViewModels.Administrator.SubCategory
{
    public class AllSubParentCategoryViewModel
    {
        public ICollection<SubCategoryViewModel> SubCategories { get; set; }

        public ICollection<ParentCategoryViewModel> ParentCategories { get; set; }
    }
}
