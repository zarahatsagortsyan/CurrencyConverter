using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyConverter.Models
{
    public class Users
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(50, ErrorMessage = "The FirstName value cannot exceed 50 characters. ")]
        public string FirstName { get; set; }

        [StringLength(50, ErrorMessage = "The LastName value cannot exceed 50 characters. ")]
        public string LastName { get; set; }

        [StringLength(50, ErrorMessage = "The Email value cannot exceed 50 characters. ")]
        public string Email { get; set; }
        public string Password { get; set; }
        //private string  HashPassword;
        //public string Password {
        //    get
        //    {
        //        return HashPassword;
        //    }
        //    set
        //    {
        //        HashPassword = BCrypt.Net.BCrypt.HashPassword(value);
        //    }
        //}

        [StringLength(3, ErrorMessage = "The BaseCur value cannot exceed 50 characters. ")]
        public string BaseCur { get; set; }
    }
}
