using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OnlineShop.Models
{
    public class BaseModel<TKey>
    {
        [Key]
        public TKey Id { get; set; }
    }
}
