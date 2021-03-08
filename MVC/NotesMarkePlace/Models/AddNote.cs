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
        public string Title { get; set; }

        public int Category { get; set; }

        public string DisplayPicture { get; set; }

        public string UploadNotes { get; set; }
        public Nullable<int> NoteType { get; set; }
        public Nullable<int> NumberofPages { get; set; }
        public string Description { get; set; }
        public string UniversityName { get; set; }
        public Nullable<int> Country { get; set; }
        public string Course { get; set; }
        public string CourseCode { get; set; }
        public string Professor { get; set; }
        public bool IsPaid { get; set; }
        public Nullable<decimal> SellingPrice { get; set; }

        public string NotesPreview { get; set; }
    }




    public class AddNoteMetaData
    {
        public string Title { get; set; }

        public int Category { get; set; }
        public string DisplayPicture { get; set; }

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
    }

   
}