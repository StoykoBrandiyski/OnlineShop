using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Web.ViewModels.Home
{
    public class IndexViewModel
    {
        public IList<IndexProductViewModel> Products { get; set; }

        public int CountAllProduct { get; set; }

        public string SearchString { get; set; }
    }
}
