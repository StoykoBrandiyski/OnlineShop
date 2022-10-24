using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShop.Models
{
    public class City : BaseModel<int>
    {
        public string Name { get; set; }

        public string Postcode { get; set; }

        public virtual ICollection<Address> Addresses { get; set; }
    }
}
