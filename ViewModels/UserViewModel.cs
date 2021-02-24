using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.Models;

namespace WebShop.ViewModels
{
    public class UserViewModel
    {
        public ApplicationUsers User { get; set; }
        public IEnumerable<Orders> Orders { get; set; }
    }
}