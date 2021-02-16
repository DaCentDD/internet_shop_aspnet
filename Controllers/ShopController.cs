using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop.Models;
using WebShop.ViewModels;

namespace WebShop.Controllers
{
    public class ShopController : Controller
    {
        public ActionResult Index(string order = "byName")
        {
            MainPageViewModel mainPageViewModel = new MainPageViewModel();
            using (ShopDbDataContext db = new ShopDbDataContext())
            {              
                mainPageViewModel.Categories = db.Categories.ToList();
                mainPageViewModel.Manufacturers = db.Manufacturers.ToList();
                mainPageViewModel.Products = db.Products;
                switch (order)
                {
                    case "byName":
                        mainPageViewModel.Products = mainPageViewModel.Products.OrderBy(p => p.Name).ToList();
                        break;
                    case "byNameDesc":
                        mainPageViewModel.Products = mainPageViewModel.Products.OrderByDescending(p => p.Name).ToList();
                        break;
                    case "byPrice":
                        mainPageViewModel.Products = mainPageViewModel.Products.OrderBy(p => p.Price).ToList();
                        break;
                    case "byPriceDesc":
                        mainPageViewModel.Products = mainPageViewModel.Products.OrderByDescending(p => p.Price).ToList();
                        break;
                }               
            }
            
            return View(mainPageViewModel);
        }

        public ActionResult Category(int id)
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult GetShopNamePartial()
        {
            using (ShopDbDataContext db = new ShopDbDataContext())
            {
                ViewBag.ShopName = db.AllNames.First(m => m.Name == "ShopName");
                return PartialView();
            }
        }

        public ActionResult GetListOfCategoriesPartial()
        {
            using (ShopDbDataContext db = new ShopDbDataContext())
            {
                ViewBag.Categories = db.Categories.ToList();
                return PartialView();
            }
        }

        public ActionResult AddToCartPartial(int productId)
        {
            if (Session["userName"] != null)
            {
                using (ShopDbDataContext db = new ShopDbDataContext())
                {
                    var cart = db.Carts.Where(c => c.UserId == (int)Session["userId"]).Where(c => c.ProductId == productId).FirstOrDefault();
                    if (cart == null)
                    {
                        db.Carts.InsertOnSubmit(new Carts
                        {
                            UserId = (int)Session["userId"],
                            ProductId = productId,
                            Quantity = 1
                        });
                    }
                    else
                    {
                        cart.Quantity += 1;

                    }
                    try
                    {
                        db.SubmitChanges();
                        ViewBag.Result = "OK";
                    }
                    catch(Exception e)
                    {
                        ViewBag.Result = e;
                    }
                }
            }
            return PartialView();
        }
    }
}