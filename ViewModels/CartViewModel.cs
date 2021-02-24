using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.Models;

namespace WebShop.ViewModels
{
    public class CartViewModel
    {
        public IEnumerable<Carts> Cart { get; set; }
        public IEnumerable<Products> Products { get; set; }
        public IEnumerable<Manufacturers> Manufacturers { get; set; }
        public int FinalSum { get; set; }
        public Dictionary<string, string> ProductsToBuy { get; set; }
    }
}