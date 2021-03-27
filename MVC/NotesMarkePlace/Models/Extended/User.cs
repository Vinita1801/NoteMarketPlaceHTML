using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NotesMarkePlace.Models
{
    [MetadataType(typeof(UserMetadata))]
    public partial class User
    {
        public string ConfirmPassword { get; set; }

        public bool RememberMe { get; set; }

        public class UserMetadata
        {

            [Required(AllowEmptyStrings = false, ErrorMessage = "First name required")]
            public string FirstName { get; set; }


            [Required(AllowEmptyStrings = false, ErrorMessage = "Last name required")]
            public string LastName { get; set; }


            [Required(AllowEmptyStrings = false, ErrorMessage = "Email ID required")]
            [DataType(DataType.EmailAddress)]
            public string EmailID { get; set; }


            [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
            [DataType(DataType.Password)]
            [MinLength(6, ErrorMessage = "Minimum 6 characters required")]
            public string Password { get; set; }


            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Confirm password and password do not match")]
            public string ConfirmPassword { get; set; }

            public bool IsEmailVerified { get; set; }
            public Nullable<System.DateTime> CreatedDate { get; set; }
            public Nullable<int> CreatedBy { get; set; }
            public Nullable<System.DateTime> ModifiedDate { get; set; }
            public Nullable<int> ModifiedBy { get; set; }
            public bool IsActive { get; set; }

            public bool RememberMe { get; set; }
            public System.Guid ActivationCode { get; set; }

            public virtual UserRole UserRole { get; set; }


        }
    }

}