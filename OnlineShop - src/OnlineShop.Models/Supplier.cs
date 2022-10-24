using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShop.Models
{
    public class Supplier : BaseModel<int>
    {
        public string Name { get; set; }

        public decimal PriceToHome { get; set; }

        public decimal PriceToOffice { get; set; }

        public bool IsDefault { get; set; }
    }
}
