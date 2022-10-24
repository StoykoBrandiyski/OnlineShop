using System.Collections.Generic;

namespace OnlineShop.Models
{
    public class ShoppingCart : BaseModel<string>
    {
        public string ShopUserId { get; set; }

        public virtual ShopUser ShopUser { get; set; }

        public virtual ICollection<ShoppingCartProduct> ShoppingCartProducts { get; set; }

    }
}