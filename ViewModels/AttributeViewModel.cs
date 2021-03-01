using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.Models;

namespace WebShop.ViewModels
{
    public class AttributeViewModel
    {
        public List<Attributes> Attributes { get; set; }
        public List<Categories> Categories { get; set; }
        public Dictionary<string, string> AttributeValuesDict { get; set; }
    }
}