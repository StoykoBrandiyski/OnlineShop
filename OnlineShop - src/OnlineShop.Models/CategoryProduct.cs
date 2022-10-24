using System.Collections.Generic;

namespace OnlineShop.Models
{
    public class CategoryProduct : BaseModel<int>
    {
        public int SubCategoryId { get; set; }

        public virtual SubCategory SubCategory { get; set; }

        public string ProductId { get; set; }

        public virtual Product Product { get; set; }
    }
}