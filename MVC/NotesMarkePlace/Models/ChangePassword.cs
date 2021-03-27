using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NotesMarkePlace.Models
{
    public class ChangePassword
    {
        [DataType(DataType.Password)]
        [Required(AllowEmptyStrings = false, ErrorMessage ="Old Password is required")]
        public string OldPassword { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "New Password is required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Minimum 6 characters required")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Confirm password and New Password do not match")]
        public string ConfirmPassword { get; set; }





    }
}