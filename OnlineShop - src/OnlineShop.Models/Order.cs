using OnlineShop.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShop.Models
{
    public class Order : BaseModel<int>
    {
        public OrderStatus Status { get; set; }

        public DateTime? OrderDate { get; set; }

        public DateTime? EstimatedDeliveryDate { get; set; }

        public DateTime? DeliveryDate { get; set; }
        
        public DateTime? DispatchDate { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public PaymentType PaymentType { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal DeliveryPrice { get; set; }

        public string Recipient { get; set; }

        public string RecipientPhoneNumber { get; set; }

        public string InvoiceNumber { get; set; }

        public string ShopUserId { get; set; }

        public ShopUser ShopUser { get; set; }

        public virtual ICollection<OrderProduct> OrderProducts { get; set; }

        public int? DeliveryAddressId { get; set; }

        public virtual Address DeliveryAddress { get; set; }
    }
}
