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
        public ActionResult Index(string search, string[] manufacturers, string order = "byName")  // Главная страница
        {
            MainPageViewModel mainPageViewModel = new MainPageViewModel();
            using (ShopDbDataContext db = new ShopDbDataContext())
            {
                mainPageViewModel.Categories = db.Categories.ToList();
                mainPageViewModel.Manufacturers = db.Manufacturers.ToList();
                mainPageViewModel.Products = db.Products;
                if (!string.IsNullOrEmpty(search))
                {
                    mainPageViewModel.Products = mainPageViewModel.Products
                        .Where(p => (p.Name.Contains(search) || p.Manufacturers.Name.Contains(search)));
                    ViewBag.search = search;
                }
                else // Если не поиск, то фильтрация
                {
                    string[] byPriceFilter = Request.Params.Get("price")?.Split(',');
                    if (byPriceFilter != null) // Фильтрация по цене
                    {
                        if (byPriceFilter[0] == "")
                            byPriceFilter[0] = "0";
                        if (byPriceFilter[1] == "")
                            byPriceFilter[1] = "9999999";
                        mainPageViewModel.Products = mainPageViewModel.Products
                            .Where(p => double.Parse(byPriceFilter[0]) <= p.Price && p.Price <= double.Parse(byPriceFilter[1]));
                    }
                    if (manufacturers != null)
                    {
                        mainPageViewModel.Products = mainPageViewModel.Products
                            .Where(p => manufacturers.Contains(p.ManufacturerId.ToString()));
                    }
                }

                switch (order)
                {
                    case "byName":
                        mainPageViewModel.Products = mainPageViewModel.Products
                            .OrderBy(p => p.Manufacturers.Name)
                            .ThenBy(p => p.Name)
                            .ToList();
                        ViewBag.byNameSelected = "selected";
                        break;
                    case "byNameDesc":
                        mainPageViewModel.Products = mainPageViewModel.Products
                            .OrderByDescending(p => p.Manufacturers.Name)
                            .ThenByDescending(p => p.Name)
                            .ToList();
                        ViewBag.byNameDescSelected = "selected";
                        break;
                    case "byPrice":
                        mainPageViewModel.Products = mainPageViewModel.Products
                            .OrderBy(p => p.Price)
                            .ToList();
                        ViewBag.byPriceSelected = "selected";
                        break;
                    case "byPriceDesc":
                        mainPageViewModel.Products = mainPageViewModel.Products
                            .OrderByDescending(p => p.Price)
                            .ToList();
                        ViewBag.byPriceDescSelected = "selected";
                        break;
                }
            }

            return View(mainPageViewModel);
        }
        public ActionResult Category(int? id, string search, string[] manufacturers, string order = "byName", params string[][] attrs)  // Страница категории
        {
            if (id == null)
                return Redirect("/Shop/Index");
            var queryStringKeys = Request.QueryString.Keys;
            Dictionary<int, string[]> attributeValuesDict = new Dictionary<int, string[]>();
            foreach (var key in queryStringKeys)  // Собираем ве значения атрибутов для фильтрации в словарь
            {
                if (int.TryParse(key.ToString(), out int attrId))  // Если ключ это Id атрибута
                {
                    string[] attrValue = Request.Params.Get(attrId.ToString()).Split(',');
                    attributeValuesDict.Add(attrId, attrValue);
                }
            }
            MainPageViewModel mainPageViewModel = new MainPageViewModel();
            using (ShopDbDataContext db = new ShopDbDataContext())
            {
                ViewBag.categoryName = db.Categories.First(c => c.Id == id).Name;
                mainPageViewModel.Manufacturers = db.Manufacturers.ToList();
                mainPageViewModel.Products = db.Products.Where(p => p.Categories.Id == id);
                mainPageViewModel.Attributes = db.Attributes
                    .Where(a => a.Categories.Id == id)
                    .ToList();
                mainPageViewModel.AttributeValues = db.AttributeValues
                    .Where(av => av.Attributes.Categories.Id == id)
                    .ToList();
                if (manufacturers != null) // Проверка на фильтрацию производителей
                {
                    mainPageViewModel.Products = mainPageViewModel.Products
                        .Where(p => manufacturers.Contains(p.ManufacturerId.ToString()));
                }
                if (!string.IsNullOrEmpty(search))  // Если производится поиск
                {
                    mainPageViewModel.Products = mainPageViewModel.Products
                        .Where(p => (p.Name.Contains(search) || p.Manufacturers.Name.Contains(search)));
                    ViewBag.search = search;
                }
                else // Иначе фильтрация
                {
                    string[] byPriceFilter = Request.Params.Get("price")?.Split(',');
                    if (byPriceFilter != null) // Фильтрация по цене
                    {
                        if (byPriceFilter[0] == "")
                            byPriceFilter[0] = "0";
                        if (byPriceFilter[1] == "")
                            byPriceFilter[1] = "9999999";
                        mainPageViewModel.Products = mainPageViewModel.Products
                            .Where(p => double.Parse(byPriceFilter[0]) <= p.Price && p.Price <= double.Parse(byPriceFilter[1]));
                    }
                    foreach (KeyValuePair<int, string[]> keyValue in attributeValuesDict)
                    {
                        try
                        {
                            if (int.TryParse(keyValue.Value[0], out _) || int.TryParse(keyValue.Value[1], out _))  // Если атрибут числовой
                            {
                                if (keyValue.Value[0] == "")
                                    keyValue.Value[0] = "0";
                                if (keyValue.Value[1] == "")
                                    keyValue.Value[1] = "9999999";
                                List<AttributeValues> attrValues = mainPageViewModel.AttributeValues
                                .Where(av => av.AttributeId == keyValue.Key)
                                .Where(av => double.Parse(keyValue.Value[0]) <= double.Parse(av.Value))
                                .Where(av => double.Parse(keyValue.Value[1]) >= double.Parse(av.Value))
                                .ToList();
                                mainPageViewModel.Products = mainPageViewModel.Products
                                    .Where(p => attrValues.Any(av => av.ProductId == p.Id));
                            }
                            else
                                throw new IndexOutOfRangeException();
                        }
                        catch (IndexOutOfRangeException) // Если значение только одно, либо атрибут не числовой
                        {
                            foreach (var value in keyValue.Value)
                            {
                                if (value == "")
                                    continue;
                                List<AttributeValues> attrValues = mainPageViewModel.AttributeValues
                                .Where(av => av.AttributeId == keyValue.Key)
                                .Where(av => av.Value == value)
                                .ToList();
                                mainPageViewModel.Products = mainPageViewModel.Products
                                    .Where(p => attrValues.Any(av => av.ProductId == p.Id));
                            }
                        }
                    }

                }

                switch (order)
                {
                    case "byName":
                        mainPageViewModel.Products = mainPageViewModel.Products
                            .OrderBy(p => p.Manufacturers.Name)
                            .ThenBy(p => p.Name)
                            .ToList();
                        ViewBag.byNameSelected = "selected";
                        break;
                    case "byNameDesc":
                        mainPageViewModel.Products = mainPageViewModel.Products
                            .OrderByDescending(p => p.Manufacturers.Name)
                            .ThenByDescending(p => p.Name)
                            .ToList();
                        ViewBag.byNameDescSelected = "selected";
                        break;
                    case "byPrice":
                        mainPageViewModel.Products = mainPageViewModel.Products
                            .OrderBy(p => p.Price)
                            .ToList();
                        ViewBag.byPriceSelected = "selected";
                        break;
                    case "byPriceDesc":
                        mainPageViewModel.Products = mainPageViewModel.Products
                            .OrderByDescending(p => p.Price)
                            .ToList();
                        ViewBag.byPriceDescSelected = "selected";
                        break;
                }
            }

            return View(mainPageViewModel);
        }

        public ActionResult GetShopNamePartial()
        {
            using (ShopDbDataContext db = new ShopDbDataContext())
            {
                ViewBag.ShopName = db.AllNames.First(m => m.Name == "ShopName");
                return PartialView();
            }
        }

        public ActionResult Product(int? id)  // Страница продукта
        {
            if (id == null)
                return Redirect("/Shop/Index");
            ProductViewModel productViewModel = new ProductViewModel();
            using (ShopDbDataContext db = new ShopDbDataContext())
            {
                productViewModel.Product = db.Products.FirstOrDefault(p => p.Id == id);
                productViewModel.Manufacturer = productViewModel.Product.Manufacturers;
                productViewModel.Category = productViewModel.Product.Categories;
                productViewModel.Attributes = db.Attributes.Where(a => a.CategoryId == productViewModel.Category.Id).ToList();
                productViewModel.AttributeValues = db.AttributeValues.Where(av => av.ProductId == id).ToList(); ;
            }
            if (productViewModel.Product == null)
                return HttpNotFound();
            return View(productViewModel);
        }

        public ActionResult Cart()
        {
            CartViewModel cartViewModel = new CartViewModel();
            using (ShopDbDataContext db = new ShopDbDataContext())
            {
                if (Session["userId"] == null)
                    return Redirect("/Account/Login/");
                cartViewModel.Cart = db.Carts.Where(c => c.UserId == (int)Session["userId"]).ToList();
                List<int> productsInCart = new List<int>();
                foreach (var cart in cartViewModel.Cart)
                {
                    productsInCart.Add(cart.ProductId);
                }
                cartViewModel.Products = db.Products.Where(p => productsInCart.Contains(p.Id)).ToList();
                cartViewModel.Manufacturers = db.Manufacturers.ToList();
            }
            return View(cartViewModel);
        }

        public ActionResult Order(int? id) //TODO
        {
            return View();
        }

        [HttpGet]
        public ActionResult Order(CartViewModel cartViewModel)
        {
            if (Session["userId"] == null)
                return Redirect("/Account/Login/");
            OrderViewModel orderViewModel = new OrderViewModel();
            if (cartViewModel.ProductsToBuy == null)
            {
                return Redirect("/shop/");
            }
            using (ShopDbDataContext db = new ShopDbDataContext())
            {
                orderViewModel.UserId = int.Parse(Session["userId"].ToString());
                orderViewModel.User = db.ApplicationUsers.First(a => a.Id == orderViewModel.UserId);
                orderViewModel.Address = orderViewModel.User.Address;
                orderViewModel.Phone = orderViewModel.User.PhoneNumber;
                orderViewModel.Price = cartViewModel.FinalSum;
                orderViewModel.ProductsToBuy = cartViewModel.ProductsToBuy;
            }
            return View(orderViewModel);
        }

        [HttpPost]
        public ActionResult Order(OrderViewModel orderViewModel)
        {
            if (ModelState.IsValid)
            {
                using (ShopDbDataContext db = new ShopDbDataContext())
                {
                    Orders newOrder = new Orders
                    {
                        UserId = orderViewModel.UserId,
                        Address = orderViewModel.Address,
                        Phone = orderViewModel.Phone,
                        Price = orderViewModel.Price,
                        DateOfOrder = DateTime.Now
                    };
                    db.Orders.InsertOnSubmit(newOrder);
                    db.SubmitChanges();
                    foreach (var product in orderViewModel.ProductsToBuy)
                    {
                        ProductOrders newProductOrder = new ProductOrders
                        {
                            OrderId = newOrder.Id,
                            ProductId = int.Parse(product.Key),
                            Quantity = int.Parse(product.Value)
                        };
                        db.ProductOrders.InsertOnSubmit(newProductOrder);
                    }
                    var cartToDelete = db.Carts.Where(c => c.UserId == orderViewModel.UserId);
                    db.Carts.DeleteAllOnSubmit(cartToDelete);
                    db.SubmitChanges();
                }
                return View("OrderDone");
            }
            else
            {
                return View(orderViewModel);
            }
        }

        public PartialViewResult GetListOfCategoriesPartial()
        {
            using (ShopDbDataContext db = new ShopDbDataContext())
            {
                ViewBag.Categories = db.Categories.ToList();
                return PartialView();
            }
        }

        public PartialViewResult AddToCartPartial(int productId)
        {
            if (Session["userName"] != null)
            {
                using (ShopDbDataContext db = new ShopDbDataContext())
                {
                    var cart = db.Carts
                        .Where(c => c.UserId == (int)Session["userId"])
                        .Where(c => c.ProductId == productId)
                        .FirstOrDefault();
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
                    catch (Exception e)
                    {
                        ViewBag.Result = e;
                    }
                }
            }
            return PartialView();
        }
    }
}