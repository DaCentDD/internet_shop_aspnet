using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WebShop.ViewModels
{
    public class LoginUserViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool CheckPassword(string passwordHashFromDb)
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
            if (passwordHash == passwordHashFromDb)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}