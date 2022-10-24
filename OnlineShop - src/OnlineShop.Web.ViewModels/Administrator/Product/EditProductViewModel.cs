using Microsoft.AspNetCore.Http;
using OnlineShop.Web.ViewModels.Administrator.SubCategory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OnlineShop.Web.ViewModels.Administrator.Product
{
    public class EditProductViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Име")]
        [Required(ErrorMessage = "Полето \"{0}\" e задължително.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Полето \"{0}\" трябва да бъде текст с минимална дължина {2} и максимална дължина {1}.")]
        public string Name { get; set; }
        
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Спецификация")]
        public string Specification { get; set; }

        [Display(Name = "Цена")]
        [Required(ErrorMessage = "Полето \"{0}\" e задължително.")]
        [Range(1, int.MaxValue, ErrorMessage = "Полето \"{0}\" трябва да е число в диапазона от {1} до {2}")]
        public decimal Price { get; set; }

        [Display(Name = "Цена за фирми")]
        [Required(ErrorMessage = "Полето \"{0}\" e задължително.")]
        [Range(1, int.MaxValue, ErrorMessage = "Полето \"{0}\" трябва да е число в диапазона от {1} до {2}")]
        public decimal ParnersPrice { get; set; }

        [Display(Name = "Категория")]
        [Required(ErrorMessage = "Полето \"{0}\" e задължително.")]
        public int SubCategoryId { get; set; }

        public ICollection<SubCategoryViewModel> SubCategories { get; set; }

        [Display(Name = "Снимки")]
        public ICollection<IFormFile> FormImages { get; set; }
    }
}
