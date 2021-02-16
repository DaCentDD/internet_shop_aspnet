using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop.Models;
using WebShop.ViewModels;

namespace WebShop.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Index(int? id)
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginUserViewModel userForCheck)
        {

            using (ShopDbDataContext db = new ShopDbDataContext())
            {
                var user = db.ApplicationUsers.First(u => u.UserName == userForCheck.UserName);
                if (user != null)
                {
                    if (userForCheck.CheckPassword(user.PasswordHash))
                    {
                        Session["userName"] = user.UserName;
                        Session["userId"] = user.Id;
                        return Redirect("/shop/index/");
                    }                   
                }
                ModelState.AddModelError("", "Неправильное имя пользователя или пароль");
            }

            return View();
        }

        public ActionResult Logout()
        {
            Session["userName"] = null;
            Session["userId"] = null;
            return Redirect("/account/login/");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterUserViewModel user)
        {
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
                            Address = user.Address
                        };
                        db.ApplicationUsers.InsertOnSubmit(newUser);
                        db.SubmitChanges();
                        return RedirectToAction("Login", "Account");
                    }
                }
            }
            else
            {
                return View();
            }

        }
    }
}
