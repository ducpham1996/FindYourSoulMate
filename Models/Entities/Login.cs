using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FindYourSoulMate.Models.Entities
{
    public class Login
    {
        [DisplayName("Email*")]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(50, ErrorMessage = "Must be between 5 and 50 characters", MinimumLength = 5)]
        [DisplayName("Password*")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        public bool remember { get; set; }
    }
}
