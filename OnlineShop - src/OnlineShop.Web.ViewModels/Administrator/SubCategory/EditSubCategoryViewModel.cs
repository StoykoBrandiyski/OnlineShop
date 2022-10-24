using Microsoft.AspNetCore.Http;
using OnlineShop.Web.ViewModels.Administrator.Category;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OnlineShop.Web.ViewModels.Administrator.SubCategory
{
    public class EditSubCategoryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Полето \"{0}\" e задължително.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Полето \"{0}\" трябва да бъде текст с минимална дължина {2} и максимална дължина {1} символа.")]
        [Display(Name = "Име")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Снимка")]
        public IFormFile FormImage { get; set; }

        [Display(Name = "Ключ за филър меню")]
        public string KeyPartial { get; set; }

        [Required(ErrorMessage = "Моля, изберете {0}.")]
        [Display(Name = "Основна категория")]
        public int ParentId { get; set; }

        public ICollection<ParentCategoryViewModel> ParentCategories { get; set; }
    }
}
