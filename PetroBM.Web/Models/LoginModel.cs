using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetroBM.Domain.Entities;
using PagedList;
using System.ComponentModel.DataAnnotations;

namespace PetroBM.Web.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage ="*")]
        [Display(Name = "Login ID")]
        public string UserName { get; set; }
        [Required(ErrorMessage ="*")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Required(ErrorMessage ="*")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPass { get; set; }
        [Required(ErrorMessage ="*")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("NewPass", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPass { get; set; }
        [Display(Name = "Remember me ?")]
        public bool RememberMe { get; set; }
    }
}