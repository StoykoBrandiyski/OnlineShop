using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShop.Models
{
    public class Product : BaseModel<string>
    {
        public string Name { get; set; }
        
        public int SubCategoryId { get; set; }

        public virtual SubCategory SubCategory { get; set; }

        public string Description { get; set; }

        public string Specification { get; set; }

        public decimal Price { get; set; }
        
        public decimal ParnersPrice { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsHide { get; set; }
        
        public virtual ICollection<ProductImage> Images { get; set; }

        public virtual ICollection<ShoppingCartProduct> ShoppingCartProducts { get; set; }

        public virtual ICollection<ShopUserFavoriteProduct> FavoriteProducts { get; set; }
    }
}
