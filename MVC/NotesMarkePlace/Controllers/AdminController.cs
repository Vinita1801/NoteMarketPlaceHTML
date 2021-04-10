using NotesMarkePlace.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace NotesMarkePlace.Controllers
{
    public class AdminController : Controller
    {
        NotesMarketPlaceEntities1 db = new NotesMarketPlaceEntities1();

        // GET: Login
        [HttpGet]
        public ActionResult login()
        {
            if (Session["UserId"] != null)
            {
                return RedirectToAction("index", new RouteValueDictionary(new { Controller = "RegisteredUsers", Action = "index" }));
            }
            else
            {
                return View();
            }

        }

        // POST : Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult login(User user, string ReturnUrl = "")
        {
            string message = "";
            using (NotesMarketPlaceEntities1 db = new NotesMarketPlaceEntities1())
            {
                User v = db.Users.Where(a => a.EmailID == user.EmailID).FirstOrDefault();

                if (v != null)
                {
                    if (!v.IsEmailVerified)
                    {
                        ViewBag.Message = "Please verify your email first";
                        return View();
                    }
                    if (string.Compare(Crypto.Hash(user.Password), v.Password) == 0)
                    {
                        int timeout = user.RememberMe ? 525600 : 20; // 525600 min = 1 year
                        var ticket = new FormsAuthenticationTicket(user.EmailID, user.RememberMe, timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);

                        Session["EmailID"] = user.EmailID;
                        Session["ID"] = v.Id;
                        var id = Session["ID"];
                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("PublishedNotes", "Admin", new { Id = id });
                        }
                    }
                    else
                    {
                        message = "*Invalid credential provided";
                    }
                }
                else
                {
                    message = "*Invalid credential provided";
                }
            }
            ViewBag.Message = message;
            return View();
        }


        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("login", "Admin");
        }

        [HttpGet]
        public ActionResult NotesUnderReview(int ? i, string sort, string search, string sellerid)
        {
            var userid = (int)Session["ID"];
            var emailid = Session["EmailId"];

            User user = new User();

          if(emailid!=null)
            {

                List<SellerNote> notesunderreview = db.SellerNotes.Where(e => e.IsActive == true && (e.Status == 2 || e.Status == 7)).OrderByDescending(e=>e.CreatedDate).ToList();

                ViewData["User"] = db.Users.ToList<User>().Select(m=>m.FirstName).Distinct().ToList();

               

               if(!String.IsNullOrEmpty(sellerid))
                {
                    notesunderreview = notesunderreview.Where(m => m.SellerId.Equals(sellerid)).ToList();
                }

                //ViewData["User"] = db.Users.Where(e => e.FirstName != null).Select(e => e.FirstName).Distinct().ToList<User>();

                if (!String.IsNullOrEmpty(search))
                {
                    notesunderreview = notesunderreview.Where(e => e.Title.ToLower().Contains(search.ToLower())).ToList();
                    
                }

                if (!String.IsNullOrEmpty(sort))
                {
                    if (sort.Equals("Title"))
                    {
                        notesunderreview = notesunderreview.OrderBy(m => m.Title).ToList();
                    }
                    if (sort.Equals("Category"))
                    {
                        notesunderreview = notesunderreview.OrderBy(m => m.Category).ToList();
                    }
                    if (sort.Equals("Status"))
                    {
                        notesunderreview = notesunderreview.OrderBy(m => m.ReferenceData.Value).ToList();
                    }
                    if (sort.Equals("Seller"))
                    {
                        notesunderreview = notesunderreview.OrderBy(m => user.FirstName).ToList();
                        notesunderreview  = notesunderreview.OrderBy(m => user.LastName).ToList();
                    }
                }

                
                return View(notesunderreview.ToPagedList(i ?? 1, 5));
            }
           
            return RedirectToAction("login", "Admin");           
        }

        [HttpGet]
        public ActionResult PublishedNotes(int? i, string sort, string search, string sellerid)
        {
            var userid = (int)Session["ID"];
            var emailid = Session["EmailId"];

            User user = new User();

            if (emailid != null)
            {

                List<SellerNote> publishednotes = db.SellerNotes.Where(e => e.IsActive == true && e.Status ==8).OrderByDescending(e => e.CreatedDate).ToList();

                ViewData["User"] = db.Users.ToList<User>().Select(m => m.FirstName).Distinct().ToList();

                if (!String.IsNullOrEmpty(sellerid))
                {
                    publishednotes = publishednotes.Where(m => m.SellerId.ToString().Equals(sellerid)).ToList();
                }

                /*SellerNote sn = new SellerNote();

                var u = db.Users.Where(e => e.Id == sn.SellerId).FirstOrDefault();

               if(!String.IsNullOrEmpty(sellerid))
                {
                    notesunderreview=notesunderreview.Where(m=>m.SellerId == u)
                }*/

                //ViewData["User"] = db.Users.Where(e => e.FirstName != null).Select(e => e.FirstName).Distinct().ToList<User>();

                if (!String.IsNullOrEmpty(search))
                {
                    publishednotes = publishednotes.Where(e => e.Title.ToLower().Contains(search.ToLower())).ToList();

                }

                if (!String.IsNullOrEmpty(sort))
                {
                    if (sort.Equals("Title"))
                    {
                        publishednotes = publishednotes.OrderBy(m => m.Title).ToList();
                    }
                    if (sort.Equals("Category"))
                    {
                        publishednotes = publishednotes.OrderBy(m => m.Category).ToList();
                    }
                    if (sort.Equals("SellType"))
                    {
                        publishednotes = publishednotes.OrderBy(m => m.NoteType).ToList();
                    }
                    if (sort.Equals("Price"))
                    {
                        publishednotes = publishednotes.OrderBy(m => m.SellingPrice).ToList();
                    }
                    if (sort.Equals("PublishedDate"))
                    {
                        publishednotes = publishednotes.OrderBy(m => m.PublishedDate).ToList();
                    }
                    if (sort.Equals("Status"))
                    {
                        publishednotes = publishednotes.OrderBy(m => m.ReferenceData.Value).ToList();
                    }
                    if (sort.Equals("Seller"))
                    {
                        publishednotes = publishednotes.OrderBy(m => user.FirstName).ToList();
                        publishednotes = publishednotes.OrderBy(m => user.LastName).ToList();
                    }
                }


                return View(publishednotes.ToPagedList(i ?? 1, 5));
            }

            return RedirectToAction("login", "Admin");
        }




        // GET: Admin
        public ActionResult AddAdministrator()
        {
            return View();
        }

        [HttpGet]
        public ActionResult NoteDetails(string noteid)
        {
            var userid = (int)Session["ID"];
            using (NotesMarketPlaceEntities1 db = new NotesMarketPlaceEntities1())
            {
                SellerNote sn = db.SellerNotes.FirstOrDefault(m => m.ID.ToString().Equals(noteid));
                if (sn != null)
                {
                    bool isvalid = db.Downloads.Any(m => m.NoteID.ToString().Equals(noteid) && m.Seller == userid && m.IsSellerHasAllowedDownload);
                    if (isvalid)
                    {
                        ViewBag.valid = "true";
                    }
                    ViewBag.Category = db.NoteCategories.Where(m => m.ID == sn.Category).FirstOrDefault().Name;
                    ViewBag.Country = db.Countries.Where(m => m.ID == sn.Country).FirstOrDefault().Name;
                    return View(sn);
                }
                else
                {
                    return HttpNotFound();
                }
            }
        }

        public FileResult Download(String path)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            string fileName = path;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        public ActionResult FreeNotes(string noteid)
        {
            var userid = (int)Session["ID"];
            using (NotesMarketPlaceEntities1 db = new NotesMarketPlaceEntities1())
            {
                bool isexist = db.Downloads.Any(m => m.NoteID.ToString().Equals(noteid) && m.Seller == userid);

                var note = db.SellerNotes.FirstOrDefault(m => m.ID.ToString().Equals(noteid));

                if (!isexist)
                {
                    Download download = new Download();
                    download.NoteID = Convert.ToInt32(noteid);
                    download.Seller = userid;
                    download.NoteTitle = note.Title;
                    download.PurchasedPrice = note.SellingPrice;
                    download.Downloader = userid;
                    download.IsSellerHasAllowedDownload = true;
                    download.IsAttachedDownloaded = true;
                    download.NoteCategory = note.NoteCategory.Name;
                    download.CreatedDate = DateTime.Now;
                    db.Downloads.Add(download);
                    db.SaveChanges();
                }
                var path = note.SellerNotesAttachements.FirstOrDefault(m => m.NoteID.ToString().Equals(noteid)).FilePath;
                return RedirectToAction("Download", "RegisteredUsers", new { path = path });
            }
            return RedirectToAction("NoteDetails", noteid);
        }

        public ActionResult PaidNotes(string noteid)
        {
            var userid = (int)Session["ID"];
            using (NotesMarketPlaceEntities1 db = new NotesMarketPlaceEntities1())
            {
                bool isexist = db.Downloads.Any(m => m.NoteID.ToString().Equals(noteid) && m.Seller == userid );
                bool isvalid = db.Downloads.Any(m => m.NoteID.ToString().Equals(noteid) && m.Seller == userid && m.IsSellerHasAllowedDownload);
                var note = db.SellerNotes.FirstOrDefault(m => m.ID.ToString().Equals(noteid));

                if (!isexist)
                {
                    Download download = new Download();
                    download.NoteID = Convert.ToInt32(noteid);
                    download.Seller = userid;
                    download.NoteTitle = note.Title;
                    download.PurchasedPrice = note.SellingPrice;
                    download.Downloader = userid;
                    download.IsSellerHasAllowedDownload = false;
                    download.IsAttachedDownloaded = true;
                    download.NoteCategory = note.NoteCategory.Name;
                    download.CreatedDate = DateTime.Now;
                    db.Downloads.Add(download);
                    db.SaveChanges();
                }
                else
                {

                    if (isvalid)
                    {

                        var path = note.SellerNotesAttachements.FirstOrDefault(m => m.NoteID.ToString().Equals(noteid)).FilePath;
                        return RedirectToAction("Download", "RegisteredUsers", new { path = path });
                    }
                }
            }
            return RedirectToAction("NoteDetails", noteid);
        }

        
        public ActionResult DownloadedNotes(int? i, string sort, string search, string sellerid,string note, string buyer, string seller)
        {
            var userid = (int)Session["ID"];
            var emailid = Session["EmailId"];


            if (emailid != null)
            {
                User user = new User();

                List<Download> downloadednotes = db.Downloads.Where(e => e.ID != null).OrderByDescending(e => e.CreatedDate).ToList();

                ViewData["Note"] = db.SellerNotes.Where(e => e.Title != null).Select(e => e.Title).Distinct().ToList();
                ViewData["Buyer"] = db.Downloads.Where(e => e.Downloader != null).Select(e => e.Downloader).Distinct().ToList();
                ViewData["Seller"] = db.Downloads.Where(e => e.Seller != null).Select(e => e.Seller).Distinct().ToList();

                List<SellerNote> notes = db.SellerNotes.Where(e => e.IsActive == true).ToList();
                List<Download> download = db.Downloads.Where(e => e.ID != null).ToList();

                if (note != "Select Note" && !String.IsNullOrEmpty(note))
                {
                    downloadednotes = downloadednotes.Where(e => e.NoteTitle != null && e.NoteTitle.ToLower().Contains(note.ToLower())).ToList();

                }

                if (seller != "Select Seller" && !String.IsNullOrEmpty(seller))
                {
                    downloadednotes = downloadednotes.Where(e => e.Seller != null && e.Seller.ToString().ToLower().Contains(seller.ToLower())).ToList();

                }

                if (buyer != "Select Buyer" && !String.IsNullOrEmpty(buyer))
                {
                    downloadednotes = downloadednotes.Where(e => e.Downloader != null && e.Downloader.ToString().ToLower().Contains(buyer.ToLower())).ToList();

                }


                if (!String.IsNullOrEmpty(sort))
                {
                    if (sort.Equals("Title"))
                    {
                         downloadednotes= downloadednotes.OrderBy(m => m.NoteTitle).ToList();
                    }
                    if (sort.Equals("Category"))
                    {
                        downloadednotes = downloadednotes.OrderBy(m => m.NoteCategory).ToList();
                    }
                   
                    if (sort.Equals("Seller"))
                    {
                        downloadednotes = downloadednotes.OrderBy(m => user.FirstName).ToList();
                       downloadednotes = downloadednotes.OrderBy(m => user.LastName).ToList();
                    }
                }


                return View(downloadednotes.ToPagedList(i ?? 1, 5));
            }

            return RedirectToAction("login", "Admin");
        }
    }
}