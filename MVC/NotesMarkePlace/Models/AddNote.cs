using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NotesMarkePlace.Models
{
    [MetadataType(typeof(AddNoteMetaData))]
    public class AddNote
    {
        public AddNote()
        {
            this.IsPaid = false;
            this.Status = 1;
            this.IsActive = true;
            this.CreatedDate = DateTime.Now;
        }

        public int ID { get; set; }
        public int SellerId { get; set; }
        public int Status { get; set; }
        public Nullable<int> ActionedBy { get; set; }
        public string AdminRemarks { get; set; }
        public Nullable<System.DateTime> PublishedDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Title required")]
        public string Title { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Category required")]
        public int Category { get; set; }

        public string DisplayPicture { get; set; }

        public HttpPostedFileBase DisplayPictureFile { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Upload Note required")]
        public string UploadNote { get; set; }

        public HttpPostedFileBase UploadNoteFile { get; set; }

        public Nullable<int> NoteType { get; set; }
        public Nullable<int> NumberofPages { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Description required")]
        public string Description { get; set; }
        public string UniversityName { get; set; }
        public Nullable<int> Country { get; set; }
        public string Course { get; set; }
        public string CourseCode { get; set; }
        public string Professor { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public bool IsPaid { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public Nullable<decimal> SellingPrice { get; set; }

        public string NotesPreview { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public HttpPostedFileBase NotesPreviewFile { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{ddd, MMM dd yyyy}")]
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }

        public bool IsActive { get; set; }

        public IEnumerable<NoteType> types { get; set; }

        public IEnumerable<NoteCategory> categories { get; set; }

        public IEnumerable<Country> countries { get; set; }
    }




    public class AddNoteMetaData
    {
        public int ID { get; set; }
        public int SellerId { get; set; }
        public int Status { get; set; }
        public Nullable<int> ActionedBy { get; set; }
        public string AdminRemarks { get; set; }
        public Nullable<System.DateTime> PublishedDate { get; set; }

        public string Title { get; set; }

        public int Category { get; set; }
        public string DisplayPicture { get; set; }

        public HttpPostedFileBase DisplayPictureFile { get; set; }

        public string UploadNote { get; set; }

        public HttpPostedFileBase UploadNoteFile { get; set; }

        public Nullable<int> NoteType { get; set; }
        public Nullable<int> NumberofPages { get; set; }
        public string Description { get; set; }

        public Nullable<int> Country { get; set; }

        public string UniversityName { get; set; }
        
        public string Course { get; set; }
        public string CourseCode { get; set; }
        public string Professor { get; set; }
        public bool IsPaid { get; set; }
        public Nullable<decimal> SellingPrice { get; set; }

        public string NotesPreview { get; set; }

        public bool IsActive { get; set; }

        public IEnumerable<NoteType> types { get; set; }

        public IEnumerable<NoteCategory> categories { get; set; }

        public IEnumerable<Country> countries { get; set; }
    }

    

}