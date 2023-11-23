using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookLoansPorject.ViewModels
{
    public class RegisterModel
    {
        [Required]
        public string Username { get; set; }
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
        [Required, DataType(DataType.Password), Compare("Password"), Display(Name ="Confirm Passwod")]
        public string ConfirmPassword { get; set; }
    }
}