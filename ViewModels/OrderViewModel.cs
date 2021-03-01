using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebShop.Models;

namespace WebShop.ViewModels
{
    public class OrderViewModel
    {
        // Для оформления
        public int UserId { get; set; }
        public ApplicationUsers User { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 50 символов")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(12, MinimumLength = 10, ErrorMessage = "Длина строки должна быть от 10 до 12 символов")]
        [RegularExpression(@"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$", ErrorMessage = "Формат номера +(code)9*********")]
        public string Phone { get; set; }
        public int Price { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0: dd/MM/yyyy/mm/HH}")]
        public DateTime DateOfOrder { get; set; }
        public Dictionary<string, string> ProductsToBuy { get; set; }

        // Для отображения деталей

        public Orders Order { get; set; }
        public IEnumerable<ProductOrders> ProductOrders { get; set; }
        public IEnumerable<Products> Products { get; set; }
        public IEnumerable<Manufacturers> Manufacturers { get; set; }
    }
}