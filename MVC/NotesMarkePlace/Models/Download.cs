//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NotesMarkePlace.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Download
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Download()
        {
            this.SellerNotesReportedIssues = new HashSet<SellerNotesReportedIssue>();
            this.SellerNotesReviews = new HashSet<SellerNotesReview>();
        }
    
        public int ID { get; set; }
        public int NoteID { get; set; }
        public int Seller { get; set; }
        public int Downloader { get; set; }
        public bool IsSellerHasAllowedDownload { get; set; }
        public string AttachmentPath { get; set; }
        public bool IsAttachedDownloaded { get; set; }
        public Nullable<System.DateTime> AttachmentDownloadedDate { get; set; }
        public bool IsPaid { get; set; }
        public Nullable<decimal> PurchasedPrice { get; set; }
        public string NoteTitle { get; set; }
        public string NoteCategory { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
    
        public virtual SellerNote SellerNote { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SellerNotesReportedIssue> SellerNotesReportedIssues { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SellerNotesReview> SellerNotesReviews { get; set; }
    }
}
