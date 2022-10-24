using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShop.Models
{
    public class ShopUserFavoriteProduct
    {
        public string ShopUserId { get; set; }
        public virtual ShopUser ShopUser { get; set; }

        public string ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
