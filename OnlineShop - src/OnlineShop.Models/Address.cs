using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShop.Models
{
    public class Address : BaseModel<int>
    {
        public Address()
        {
            this.Addresses = new HashSet<Order>();
        }

        public string Country { get; set; }

        public int CityId { get; set; }

        public virtual City City { get; set; }

        public string Description { get; set; }

        public string ShopUserId { get; set; }

        public virtual ShopUser ShopUser { get; set; }

        public string Street { get; set; }

        public string BuildingNumber { get; set; }

        public virtual ICollection<Order> Addresses { get; set; }
    }
}
