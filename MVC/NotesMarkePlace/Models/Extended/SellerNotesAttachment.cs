using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarkePlace.Models
{
    public class SellerNotesAttachment
    {
        public SellerNotesAttachment()
        {
            this.CreatedDate = DateTime.Now;
            this.CreatedBy = 3;
            this.IsActive = true;
        }


        public int ID { get; set; }
        public int NoteID { get; set; }
        public string FileName { get; set; }

        public HttpPostedFileBase FileNameFile { get; set; }
        public string FilePath { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public bool IsActive { get; set; }

        public virtual SellerNote SellerNote { get; set; }
    }
}