using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using WebShop.Models;

namespace WebShop.ViewModels
{
    public class RegisterUserViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 50 символов")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 50 символов")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(12, MinimumLength = 10, ErrorMessage = "Длина строки должна быть от 10 до 12 символов")]
        [RegularExpression(@"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$", ErrorMessage = "Формат номера +(code)9*********")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 50 символов")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public bool IsAdmin { get; set; } = false;
        public List<string> Validate(ShopDbDataContext db)
        {
            List<string> errors = new List<string>();
            if (db.ApplicationUsers.Any(u => u.UserName == this.UserName))  // Валидация UserName
            {
                errors.Add("Такой пользователь уже существует");
            }
            if (this.Password.Length < 6)  // Валидация длины пароля
            {
                errors.Add("Длина пароля должна быть больше 6 символов");
            }
            return errors;
        }

        public string GetPasswordHash()
        {
            string passwordHash;
            using (MD5 hash = MD5.Create())
            {
                passwordHash = String.Join
                (
                    "",
                    from ba in hash.ComputeHash
                    (
                        Encoding.UTF8.GetBytes(this.Password)
                    )
                    select ba.ToString("x2")
                );
            }
            return passwordHash;
        }
    }
}