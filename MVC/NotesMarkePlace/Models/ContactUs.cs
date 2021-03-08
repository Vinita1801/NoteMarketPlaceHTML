using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NotesMarkePlace.Models
{
    [MetadataType(typeof(ContactUsMetaData))]
    public class ContactUs
    {
        public string FullName { get; set; }

        public string EmailId { get; set; }

        public string Subject { get; set; }

        public string Comments { get; set; }
    }

    public class ContactUsMetaData
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Full Name required")]
        public string FullName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Email Address required")]
        public string EmailId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Subject required")]
        public string Subject { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Comments required")]
        public string Comments { get; set; }

    }
}