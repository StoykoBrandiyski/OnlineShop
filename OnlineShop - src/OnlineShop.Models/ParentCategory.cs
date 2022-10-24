using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShop.Models
{
    public class ParentCategory : BaseModel<int>
    {
        public string Name { get; set; }

        public virtual ICollection<SubCategory> SubCategories { get; set; }  
    }
}
