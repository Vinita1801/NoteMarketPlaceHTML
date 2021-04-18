using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NotesMarkePlace.Models
{
    public class userprofile
    {
        public int ID { get; set; }
        public int UserID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name required")]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name required")]
        public string LastName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Email Id required")]
        public string EmailId { get; set; }

        public Nullable<System.DateTime> DOB { get; set; }
        public Nullable<int> Gender { get; set; }
        public string SecondaryEmailAddress { get; set; }

       
        public string PhoneNumber_CountryCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Phone Number required")]
        public string PhoneNumber { get; set; }
        public string ProfilePicture { get; set; }

        public HttpPostedFileBase ProfilePictureFile { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "AddressLine1 required")]
        public string AddressLine1 { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "AddressLine2 required")]
        public string AddressLine2 { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "City required")]
        public string City { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "State required")]
        public string State { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Zip Code required")]
        public string ZipCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Country required")]
        public string Country { get; set; }
        public string University { get; set; }
        public string College { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }

        public virtual ReferenceData ReferenceData { get; set; }

    }
}