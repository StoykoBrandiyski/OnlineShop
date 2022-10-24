
using OnlineShop.Web.ViewModels.ShoppingCart;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OnlineShop.Web.ViewModels.Order
{
    public class CreateOrderViewModel
    {
        public IList<OrderAdressViewModel> OrderAddressesViewModel { get; set; }

        public OrderAdressViewModel OrderAdressViewModel { get; set; }
        
        //[Required(ErrorMessage = "Полето \"{0}\" e задължително.")]
        //public int SupplierId { get; set; }

        [Display(Name = "Адрес на получаване")]
        [Required(ErrorMessage = "Моля изберете \"{0}\".")]
        public int? DeliveryAddressId { get; set; }

        [Display(Name = "Име на получателя")]
        [Required(ErrorMessage = "Моля въведете \"{0}\".")]
        public string FullName { get; set; }

        [Display(Name = "Телефонен номер")]
        [Required(ErrorMessage = "Моля въведете \"{0}\".")]
        public string PhoneNumber { get; set; }

        //[Display(Name = "Начин на плащане")]
        //[Required(ErrorMessage = "Моля изберете \"{0}\".")]
        //public PaymentType PaymentType { get; set; }

        public ICollection<ShoppingCartProductsViewModel> Products { get; set; }
    }
}
