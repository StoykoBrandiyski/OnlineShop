using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShop.Models
{
    public class Company : BaseModel<int>
    {
        public string Name { get; set; }

        public string Manager { get; set; }

        public string Owner { get; set; }

        public DateTime RegistrationDate { get; set; }
        
        public virtual Address Address { get; set; }
       
        public virtual ShopUser ShopUser { get; set; }
    }
}
