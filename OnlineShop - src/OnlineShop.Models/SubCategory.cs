using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShop.Models
{
    public class SubCategory : BaseModel<int>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string KeyPartial { get; set; }

        public virtual string ImageUrl { get; set; }

        public int ParentCategoryId { get; set; }

        public virtual ParentCategory ParentCategory { get; set; }

        public virtual ICollection<Product> Products { get; set; }

    }
}
