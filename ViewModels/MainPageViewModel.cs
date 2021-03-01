using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.Models;

namespace WebShop.ViewModels
{
    public class MainPageViewModel
    {
        public IEnumerable<Categories> Categories { get; set; }
        public IEnumerable<Products> Products { get; set; }
        public IEnumerable<Attributes> Attributes { get; set; }
        public IEnumerable<AttributeValues> AttributeValues { get; set; }
        public IEnumerable<Manufacturers> Manufacturers { get; set; }

        public string GetManufacturer(int? ManufacturerId)
        {
            if (ManufacturerId == null)
                return "Неизвестно";
            return Manufacturers.First(m => m.Id == ManufacturerId).Name;
        }
    }
}