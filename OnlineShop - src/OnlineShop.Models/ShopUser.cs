using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace OnlineShop.Models
{
    public class ShopUser :IdentityUser
    {
        public ShopUser()
        {
            this.Addresses = new HashSet<Address>();
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        
        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public string ShoppingCartId { get; set; }

        //Fix ShppingCart
        public virtual ShoppingCart ShoppingCart { get; set; }

        public virtual ICollection<Address> Addresses { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

        public virtual ICollection<ShopUserFavoriteProduct> FavoriteProducts { get; set; }
    }
}
