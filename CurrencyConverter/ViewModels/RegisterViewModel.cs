using CurrencyConverter.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyConverter.ViewModels
{
    public class RegisterViewModel
    {
        public Guid Id { get; set; }

        [StringLength(50, ErrorMessage = "The FirstName value cannot exceed 50 characters. ")]
        public string FirstName { get; set; }

        [StringLength(50, ErrorMessage = "The LastName value cannot exceed 50 characters. ")]
        public string LastName { get; set; }

        [StringLength(50, ErrorMessage = "The Email value cannot exceed 50 characters. ")]
        public string Email { get; set; }

        [StringLength(50, ErrorMessage = "The Password value cannot exceed 50 characters. ")]
        public string Password { get; set; }

        [StringLength(50, ErrorMessage = "The Password value cannot exceed 50 characters. ")]
        public string ConfirmPassword { get; set; }

        [StringLength(3, ErrorMessage = "The BaseCur value cannot exceed 50 characters. ")]
        public string BaseCur { get; set; }
    }
}
