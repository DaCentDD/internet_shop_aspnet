using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop.ViewModels;
using WebShop.Models;
using System.Web.Helpers;

namespace WebShop.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            if (Session["userId"] == null || Session["isAdmin"].ToString() == false.ToString())
                return RedirectToAction("Index", "Shop");
            return View();
        }

        [HttpGet]
        [Route("user/add/", Name = "UserAddRoute")]
        public ActionResult UserAdd()
        {
            if (Session["userId"] == null || Session["isAdmin"].ToString() == false.ToString())
                return RedirectToAction("Index", "Shop");
            return View();
        }

        [HttpPost]
        [Route("user/add/")]
        public ActionResult UserAdd(RegisterUserViewModel user)
        {
            if (Session["userId"] == null || Session["isAdmin"].ToString() == false.ToString())
                return RedirectToAction("Index", "Shop");
            if (ModelState.IsValid)
            {
                using (ShopDbDataContext db = new ShopDbDataContext())
                {
                    var validateResult = user.Validate(db);
                    if (validateResult.Count() > 0)  // В случае неуспешной валидации выводим ошибки
                    {
                        foreach (string error in validateResult)
                        {
                            ModelState.AddModelError("", error);
                        }
                        return View();
                    }
                    else
                    {
                        ApplicationUsers newUser = new ApplicationUsers
                        {
                            UserName = user.UserName,
                            PasswordHash = user.GetPasswordHash(),
                            Email = user.Email,
                            PhoneNumber = user.PhoneNumber,
                            Address = user.Address,
                            IsAdmin = user.IsAdmin
                        };
                        db.ApplicationUsers.InsertOnSubmit(newUser);
                        db.SubmitChanges();
                        return RedirectToAction("Index", "Admin");
                    }
                }
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        [Route("user/edit/", Name = "UserEditRoute")]
        public ActionResult UserEdit()
        {
            if (Session["userId"] == null || Session["isAdmin"].ToString() == false.ToString())
                return RedirectToAction("Index", "Shop");
            List<ApplicationUsers> users;
            using (ShopDbDataContext db = new ShopDbDataContext())
            {
                users = db.ApplicationUsers.ToList();
            }
            return View(users);
        }

        [HttpPost]
        [Route("user/edit/")]
        public RedirectToRouteResult UserEdit(RegisterUserViewModel user)
        {
            if (Session["userId"] == null || Session["isAdmin"].ToString() == false.ToString())
                return RedirectToAction("Index", "Shop");
            using (ShopDbDataContext db = new ShopDbDataContext())
            {
                var userToEdit = db.ApplicationUsers.First(u => u.Id == user.Id);
                userToEdit.UserName = user.UserName;
                userToEdit.Email = user.Email;
                userToEdit.PhoneNumber = user.PhoneNumber;
                userToEdit.Address = user.Address;
                userToEdit.IsAdmin = user.IsAdmin;
                db.SubmitChanges();
            }
            return RedirectToAction("UserEdit", "Admin");
        }

        [HttpGet]
        [Route("user/delete/", Name = "UserDeleteRoute")]
        public ActionResult UserDelete()
        {
            if (Session["userId"] == null || Session["isAdmin"].ToString() == false.ToString())
                return RedirectToAction("Index", "Shop");
            List<ApplicationUsers> users;
            using (ShopDbDataContext db = new ShopDbDataContext())
            {
                users = db.ApplicationUsers.ToList();
            }
            return View(users);
        }

        [HttpPost]
        [Route("user/delete/")]
        public RedirectToRouteResult UserDelete(int? id)
        {
            if (Session["userId"] == null || Session["isAdmin"].ToString() == false.ToString())
                return RedirectToAction("Index", "Shop");
            if (id != null)
            {
                using (ShopDbDataContext db = new ShopDbDataContext())
                {
                    var userToDelete = db.ApplicationUsers.First(u => u.Id == id);
                    db.ApplicationUsers.DeleteOnSubmit(userToDelete);
                    db.SubmitChanges();
                    return RedirectToRoute("UserDeleteRoute");
                }
            }
            return RedirectToAction("UserDelete", "Admin");
        }

        [HttpGet]
        [Route("user/edit/password/{id}")]
        public ActionResult PasswordEdit(int id)
        {
            RegisterUserViewModel registerUserViewModel = new RegisterUserViewModel
            {
                Id = id
            };
            return View(registerUserViewModel);
        }

        [HttpPost]
        [Route("user/edit/password/{id}")]
        public RedirectToRouteResult PasswordEdit(RegisterUserViewModel registerUserViewModel)
        {
            using (ShopDbDataContext db = new ShopDbDataContext())
            {
                var userToChangePassword = db.ApplicationUsers.First(u => u.Id == registerUserViewModel.Id);
                userToChangePassword.PasswordHash = registerUserViewModel.GetPasswordHash();
                db.SubmitChanges();
            }
            return RedirectToAction("UserEdit", "Admin");

        }

        [HttpGet]
        [Route("category/edit/", Name = "CategoryEditRoute")]
        public ActionResult CategoryEdit()
        {
            List<Categories> categories;
            using (ShopDbDataContext db = new ShopDbDataContext())
            {
                categories = db.Categories.ToList();
            }
            return View(categories);
        }

        [HttpPost]
        [Route("category/edit/")]
        public RedirectToRouteResult CategoryEdit(int id, string name)
        {
            using (ShopDbDataContext db = new ShopDbDataContext())
            {
                var categoryToEdit = db.Categories.First(c => c.Id == id);
                categoryToEdit.Name = name;
                db.SubmitChanges();
            }
            return RedirectToAction("CategoryEdit", "Admin");
        }

        [HttpPost]
        [Route("category/delete/", Name = "CategoryDeleteRoute")]
        public RedirectToRouteResult CategoryDelete(int id)
        {
            using (ShopDbDataContext db = new ShopDbDataContext())
            {
                var categoryToDelete = db.Categories.First(c => c.Id == id);
                db.Categories.DeleteOnSubmit(categoryToDelete);
                db.SubmitChanges();
            }
            return RedirectToAction("CategoryEdit", "Admin");
        }

        [HttpPost]
        [Route("category/create/", Name = "CategoryCreateRoute")]
        public RedirectToRouteResult CategoryCreate(string name)
        {
            using (ShopDbDataContext db = new ShopDbDataContext())
            {
                Categories newCategory = new Categories
                {
                    Name = name
                };
                db.Categories.InsertOnSubmit(newCategory);
                db.SubmitChanges();
            }
            return RedirectToAction("CategoryEdit", "Admin");
        }

        [HttpGet]
        [Route("attribute/edit/", Name = "AttributeEditRoute")]
        public ActionResult AttributeEdit()
        {
            AttributeViewModel attributeViewModel = new AttributeViewModel();
            using (ShopDbDataContext db = new ShopDbDataContext())
            {
                attributeViewModel.Attributes = db.Attributes.ToList();
                attributeViewModel.Categories = db.Categories.ToList();
            }
            return View(attributeViewModel);
        }

        [HttpPost]
        [Route("attribute/edit/")]
        public RedirectToRouteResult AttributeEdit(int id, string name, int categoryId)
        {
            using (ShopDbDataContext db = new ShopDbDataContext())
            {
                var attributeToEdit = db.Attributes.First(c => c.Id == id);
                attributeToEdit.Name = name;
                attributeToEdit.CategoryId = categoryId;
                db.SubmitChanges();
            }
            return RedirectToAction("AttributeEdit", "Admin");
        }

        [HttpPost]
        [Route("attribute/delete/", Name = "AttributeDeleteRoute")]
        public RedirectToRouteResult AttributeDelete(int id)
        {
            using (ShopDbDataContext db = new ShopDbDataContext())
            {
                var attributeToDelete = db.Attributes.First(c => c.Id == id);
                db.Attributes.DeleteOnSubmit(attributeToDelete);
                db.SubmitChanges();
            }
            return RedirectToAction("AttributeEdit", "Admin");
        }

        [HttpPost]
        [Route("attribute/create/", Name = "AttributeCreateRoute")]
        public RedirectToRouteResult AttributeCreate(string name, int categoryId)
        {
            using (ShopDbDataContext db = new ShopDbDataContext())
            {
                Attributes newAttribute = new Attributes
                {
                    Name = name,
                    CategoryId = categoryId
                };
                db.Attributes.InsertOnSubmit(newAttribute);
                db.SubmitChanges();
            }
            return RedirectToAction("AttributeEdit", "Admin");
        }

        [HttpGet]
        [Route("manufacturer/edit/", Name = "ManufacturerEditRoute")]
        public ActionResult ManufacturerEdit()
        {
            List<Manufacturers> manufacturers;
            using (ShopDbDataContext db = new ShopDbDataContext())
            {
                manufacturers = db.Manufacturers.ToList();
            }
            return View(manufacturers);
        }

        [HttpPost]
        [Route("manufacturer/edit/")]
        public RedirectToRouteResult ManufacturerEdit(int id, string name)
        {
            using (ShopDbDataContext db = new ShopDbDataContext())
            {
                var manufacturerToEdit = db.Manufacturers.First(c => c.Id == id);
                manufacturerToEdit.Name = name;
                db.SubmitChanges();
            }
            return RedirectToAction("ManufacturerEdit", "Admin");
        }

        [HttpPost]
        [Route("manufacturer/delete/", Name = "ManufacturerDeleteRoute")]
        public RedirectToRouteResult ManufacturerDelete(int id)
        {
            using (ShopDbDataContext db = new ShopDbDataContext())
            {
                var manufacturerToDelete = db.Manufacturers.First(c => c.Id == id);
                db.Manufacturers.DeleteOnSubmit(manufacturerToDelete);
                db.SubmitChanges();
            }
            return RedirectToAction("ManufacturerEdit", "Admin");
        }


        [HttpPost]
        [Route("manufacturer/create/", Name = "ManufacturerCreateRoute")]
        public RedirectToRouteResult ManufacturerCreate(string name)
        {
            using (ShopDbDataContext db = new ShopDbDataContext())
            {
                Manufacturers newManufacturer = new Manufacturers
                {
                    Name = name
                };
                db.Manufacturers.InsertOnSubmit(newManufacturer);
                db.SubmitChanges();
            }
            return RedirectToAction("ManufacturerEdit", "Admin");
        }

        [HttpGet]
        [Route("product/edit/", Name = "ProductEditRoute")]
        public ActionResult ProductEdit()
        {
            List<ProductViewModel> productViewModels = new List<ProductViewModel>();
            using (ShopDbDataContext db = new ShopDbDataContext())
            {
                foreach (var product in db.Products)
                {
                    ProductViewModel productViewModel = new ProductViewModel
                    {
                        Product = product,
                        Manufacturer = product.Manufacturers,
                        Category = product.Categories,
                        Attributes = db.Attributes.Where(a => a.CategoryId == product.CategoryId).ToList(),
                        AttributeValues = db.AttributeValues.Where(av => av.ProductId == product.Id).ToList(),
                    };
                    productViewModels.Add(productViewModel);
                }
                ViewBag.Manufacturers = db.Manufacturers.ToList();
                ViewBag.Categories = db.Categories.ToList();
            }

            return View(productViewModels);
        }

        [HttpPost]
        [Route("product/edit/")]
        public RedirectToRouteResult ProductEdit(Products editedProduct, HttpPostedFileBase Image)
        {
            using (ShopDbDataContext db = new ShopDbDataContext())
            {
                var productToEdit = db.Products.First(c => c.Id == editedProduct.Id);
                productToEdit.Name = editedProduct.Name;
                productToEdit.ManufacturerId = editedProduct.ManufacturerId;
                productToEdit.CategoryId = editedProduct.CategoryId;
                productToEdit.Price = editedProduct.Price;
                if (Image != null)
                {
                    string newFileName = System.IO.Path.GetFileName(Image.FileName);
                    productToEdit.Image = newFileName;
                    Image.SaveAs(Server.MapPath("~/Content/Img/" + newFileName));
                }
                db.SubmitChanges();
                return RedirectToAction("AttributeValueEdit", "Admin", new { productId = productToEdit.Id });
            }
        }

        [HttpPost]
        [Route("product/delete/", Name = "ProductDeleteRoute")]
        public RedirectToRouteResult ProductDelete(int id)
        {
            using (ShopDbDataContext db = new ShopDbDataContext())
            {
                var productToDelete = db.Products.First(c => c.Id == id);
                db.Products.DeleteOnSubmit(productToDelete);
                db.SubmitChanges();
            }
            return RedirectToAction("ProductEdit", "Admin");
        }

        [HttpPost]
        [Route("product/create/", Name = "ProductCreateRoute")]
        public RedirectToRouteResult ProductCreate(Products newProduct, HttpPostedFileBase Image)
        {
            if (Image != null)
            {
                string newFileName = System.IO.Path.GetFileName(Image.FileName);
                using (ShopDbDataContext db = new ShopDbDataContext())
                {
                    newProduct.Image = newFileName;
                    db.Products.InsertOnSubmit(newProduct);
                    db.SubmitChanges();
                    Image.SaveAs(Server.MapPath("~/Content/Img/" + newFileName));
                }
            }
            return RedirectToAction("AttributeValueEdit", "Admin", new { productId = newProduct.Id });
        }


        [HttpGet]
        [Route("attributevalue/create/")]
        public ActionResult AttributeValueEdit(int productId)
        {
            ProductViewModel productViewModel = new ProductViewModel();
            using (ShopDbDataContext db = new ShopDbDataContext())
            {
                productViewModel.Product = db.Products.First(p => p.Id == productId);
                productViewModel.Manufacturer = db.Manufacturers.First(m => m.Id == productViewModel.Product.ManufacturerId);
                productViewModel.Category = db.Categories.First(c => c.Id == productViewModel.Product.CategoryId);
                productViewModel.Attributes = db.Attributes.Where(a => a.CategoryId == productViewModel.Category.Id).ToList();
                productViewModel.AttributeValues = db.AttributeValues.Where(av => av.ProductId == productId).ToList();
            }
            return View(productViewModel);
        }

        [HttpPost]
        [Route("attributevalue/create/")]
        public RedirectToRouteResult AttributeValueEdit(int productId, AttributeViewModel attributeViewModel)
        {
            using (ShopDbDataContext db = new ShopDbDataContext())
            {
                foreach (KeyValuePair<string, string> attrVal in attributeViewModel.AttributeValuesDict)
                {
                    var ifExistAttrValue = db.AttributeValues.First(av => av.ProductId == productId && av.AttributeId == int.Parse(attrVal.Key));
                    if (ifExistAttrValue != null)
                    {
                        ifExistAttrValue.Value = attrVal.Value;
                        db.SubmitChanges();
                    }
                    else
                    {
                        AttributeValues attributeValues = new AttributeValues
                        {
                            ProductId = productId,
                            AttributeId = int.Parse(attrVal.Key),
                            Value = attrVal.Value
                        };
                        db.AttributeValues.InsertOnSubmit(attributeValues);
                        db.SubmitChanges();
                    }
                }
            }
            return RedirectToAction("ProductEdit", "Admin");
        }
    }
}