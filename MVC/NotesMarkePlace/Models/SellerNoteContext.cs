using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace NotesMarkePlace.Models
{
    public class SellerNoteContext:DbContext
    {
        public DbSet<SellerNote> Notes { get; set; }
    }
}