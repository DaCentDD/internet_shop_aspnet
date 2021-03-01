using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.Models;

namespace WebShop.ViewModels
{
    public class ProductViewModel
    {
        public Products Product { get; set; }
        public Manufacturers Manufacturer { get; set; }
        public Categories Category { get; set; }
        public IEnumerable<Attributes> Attributes { get; set; }
        public IEnumerable<AttributeValues> AttributeValues { get; set; }
        

    }
}