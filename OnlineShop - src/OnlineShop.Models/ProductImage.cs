using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShop.Models
{
    public class ProductImage : BaseModel<string>
    {
        public string ImageUrl { get; set; }

        public string ProductId { get; set; }

        public virtual Product Product { get; set; }
    }
}
