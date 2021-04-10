using NotesMarkePlace.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.WebPages;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Routing;
using System.IO;
using PagedList;
using System.Configuration;
using System.Data.Entity.Validation;

namespace NotesMarkePlace.Controllers
{

    public class RegisteredUsersController : Controller
    {
        int temp = 0;

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
                            return RedirectToAction("BuyerRequests","RegisteredUsers",new { Id=id});
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


        // GET: Signup
        [HttpGet]
        public ActionResult signup()
        {
            return View();
        }


        // POST: Signup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult signup([Bind(Exclude = "IsEmailVerified,ActivationCode")] User user)
        {
            bool Status = false;
            string message = "";

            // Model Validation
            if (ModelState.IsValid)
            {
                #region 
                // Email is already Exist 
                var isExist = IsEmailExist(user.EmailID);

                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    return View(user);
                }
                #endregion


                user.RoleID = 1;
                #region 
                //Generate Activation Code 
                user.ActivationCode = Guid.NewGuid();
                #endregion

                #region
                // Password Hashing
                user.Password = Crypto.Hash(user.Password);
                user.ConfirmPassword = Crypto.Hash(user.ConfirmPassword);
                #endregion

                user.IsEmailVerified = false;

                #region
                // Save To Database
                using (NotesMarketPlaceEntities1 db = new NotesMarketPlaceEntities1())
                {

                    db.Users.Add(user);
                    db.SaveChanges();

                    // Send Email to User
                    SendVerificationLinkEmail(user.FirstName, user.LastName, user.EmailID, user.ActivationCode.ToString(), "VerifyAccount");
                    message = "Registration successfully done. Account activation link " +
                        "has been sent to your email id:" + user.EmailID;
                    Status = true;
                }
                #endregion
            }
            else
            {
                message = "Invalid Request";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(user);
        }

        [NonAction]
        public bool IsEmailExist(string emailID)
        {
            using (NotesMarketPlaceEntities1 db = new NotesMarketPlaceEntities1())
            {
                var v = db.Users.Where(a => a.EmailID == emailID).FirstOrDefault();
                return v != null;
            }
        }

        [NonAction]
        public void SendVerificationLinkEmail(string firstname, string lastname, string emailId, string activationCode, string EmailFor)
        {
            var verifyUrl = "/RegisteredUsers/EmailVerification/" + activationCode;
            var link = "https://localhost:44379/" + verifyUrl;

            var fromEmail = new MailAddress("notesmarketplacesrs@gmail.com", "Notes MarkePlace");
            var toEmail = new MailAddress(emailId);
            var fromEmailPassword = "NotesMP@123"; // Replace with actual password

            string subject = "";
            string body = "";

            if (EmailFor == "VerifyAccount")
            {
                subject = "Note Marketplace - Email Verification";

                body = "<br/><br/> Hello " + firstname + " " + lastname + "," +
                    "<br/><br/>Thank you for signing up with us. Please click on below link to verify your email address and to do login." +
                    "<br/><br/><a href='" + link + "'>" + link + "</a><br/><br/> Regards,<br/> Notes Marketplace ";
            }

            else if (EmailFor == "ResetPassword")
            {
                subject = "New Temporary Password has been created for you";
                body = "<br/><br/> Hello, <br/><br/> We have generated a  new password for you <br/> Password:" + activationCode;
            }

            

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true

            })
                smtp.Send(message);
        }

        // GET : Email Verification
        [HttpGet]
        public ActionResult EmailVerification(String id)
        {
            using (NotesMarketPlaceEntities1 db = new NotesMarketPlaceEntities1())
            {
                // This line I have added here to avoid  Confirm password does not match issue on save changes
                db.Configuration.ValidateOnSaveEnabled = false;
                var v = db.Users.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();

                if (v != null)
                {
                    return View(v);
                }
                else
                {
                    TempData["ErrorMessage"] = "Account not found";
                    return RedirectToAction("signup", "RegisteredUsers");
                }
            }
        }

        // POST : Email Verification
        [HttpPost]
        public ActionResult EmailVerification(User user)
        {
            using (NotesMarketPlaceEntities1 db = new NotesMarketPlaceEntities1())
            {
                User user1 = db.Users.Where(x => x.Id == user.Id).FirstOrDefault();
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Account not found";
                    return RedirectToAction("signup", "RegisteredUsers");
                }
                else
                {
                    user1.IsEmailVerified = true;
                    db.SaveChanges();
                    TempData["SucessMessage"] = "Your account has been Verified";
                    return RedirectToAction("login", "RegisteredUsers");
                }
            }
        }


        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("login", "RegisteredUsers");
        }



        //GET : Index

        [HttpGet]
        public ActionResult index(string username,int Id)
        {
            ViewBag.username = username;
            return View();
        }

        [HttpGet]
        public ActionResult forgotPassword()
        {
            return View();
        }


        [HttpPost]
        public ActionResult forgotPassword(string EmailId, User user)
        {
            string message = "";
            bool Status = false;

            using (NotesMarketPlaceEntities1 db = new NotesMarketPlaceEntities1())
            {
                var account = db.Users.Where(a => a.EmailID == EmailId).FirstOrDefault();
                if (account != null)
                {
                    String Password = Membership.GeneratePassword(12, 0);
                    account.Password = Crypto.Hash(Password);
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    SendVerificationLinkEmail(null, null, account.EmailID, Password, "ResetPassword");
                }
            }

            return View();
        }



        [HttpGet]
        public ActionResult ContactUs()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ContactUs(ContactUs cs)
        {
            Query(cs.FullName, cs.EmailId, cs.Comments);
            return View(cs);
        }

        [NonAction]
        public void Query(string fullname, string email, string comments)
        {
            var fromEmail = new MailAddress("notesmarketplacesrs@gmail.com", "Notes MarkePlace");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "NotesMP@123"; // Replace with actual password
            string subject = fullname + " - Query";

            string body = "<br/><br/> Hello <br/><br/>" + comments +
                "<br/><br/> Regards, <br/>" + fullname;


            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true

            })
                smtp.Send(message);
        }

        [HttpGet]
        public ActionResult addNote()
        {

            AddNote a = new AddNote
            {
                 types = GetNoteTypes(),
                 categories = GetNoteCategories(),
                 countries = GetCountryList()
            };
            return View(a);
        }

        public List<NoteType> GetNoteTypes()
        {
            using (NotesMarketPlaceEntities1 db = new NotesMarketPlaceEntities1())
            {
                List<NoteType> noteTypes = db.NoteTypes.ToList();
                return noteTypes;
            }

        }

        public List<NoteCategory> GetNoteCategories()
        {
            using (NotesMarketPlaceEntities1 db = new NotesMarketPlaceEntities1())
            {
                List<NoteCategory> noteCategories = db.NoteCategories.ToList();
                return noteCategories;
            }

        }

        public List<Country> GetCountryList()
        {
            using (NotesMarketPlaceEntities1 db = new NotesMarketPlaceEntities1())
            {
                List<Country> country = db.Countries.ToList();
                return country;
            }

        }


        [HttpPost]
        public ActionResult addNote(AddNote addnote,SellerNotesAttachment sellernoteattach)
        {
           using(NotesMarketPlaceEntities1 db = new NotesMarketPlaceEntities1())
            {
                SellerNote sellernote = new SellerNote();

                //Need to change after session management
                sellernote.SellerId = 1;

                sellernote.Status = 1;

                sellernote.ActionedBy = 1;

                sellernote.Title = addnote.Title;

                sellernote.Category = addnote.Category;

                if (addnote.DisplayPictureFile != null)
                {
                    string FileName = Path.GetFileNameWithoutExtension(addnote.DisplayPictureFile.FileName);

                    string FileExtension = Path.GetExtension(addnote.DisplayPictureFile.FileName);

                    FileName = DateTime.Now.ToString("yyyyMMdd") + FileName.Trim() + FileExtension;

                    sellernote.DisplayPicture = FileName;

                    FileName = Path.Combine(Server.MapPath("~/UploadedFiles/") + FileName);

                    addnote.DisplayPictureFile.SaveAs(FileName);

           
                }
                
                sellernote.NoteType = addnote.NoteType;

                sellernote.NumberofPages = addnote.NumberofPages;

                sellernote.Description = addnote.Description;

                sellernote.Country = addnote.Country;

                sellernote.UniversityName = addnote.UniversityName;

                sellernote.Course = addnote.Course;

                sellernote.CourseCode = addnote.CourseCode;

                sellernote.Professor = addnote.Professor;

                sellernote.IsPaid = addnote.IsPaid;

                sellernote.SellingPrice = addnote.SellingPrice;

                if(addnote.NotesPreviewFile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(addnote.NotesPreviewFile.FileName);

                    string fileExtension = Path.GetExtension(addnote.NotesPreviewFile.FileName);

                    fileName = DateTime.Now.ToString("yyyyMMdd") + fileName.Trim() + fileExtension;

                    sellernote.NotesPreview = fileName;

                    fileName = Path.Combine(Server.MapPath("~/UploadedFiles/") + fileName);

                    addnote.NotesPreviewFile.SaveAs(fileName);

                }

                db.SellerNotes.Add(sellernote);
                db.SaveChanges();

                SellerNotesAttachement sellernotesattachment = new SellerNotesAttachement();

                SellerNote sn = db.SellerNotes.Where(e => e.Title == sellernote.Title).FirstOrDefault();
                sellernotesattachment.NoteID = sn.ID;
                

                if(addnote.UploadNoteFile!= null)
                {
                    string FileName = Path.GetFileNameWithoutExtension(addnote.UploadNoteFile.FileName);

                    string FilePath = Path.GetExtension(addnote.UploadNoteFile.FileName);

                    sellernotesattachment.FileName = FileName;

                    FileName = DateTime.Now.ToString("yyyyMMdd") + FileName.Trim() + FilePath;

                    FileName = Path.Combine(Server.MapPath("~/UploadedFiles/") + FileName);

                    sellernotesattachment.FilePath = FileName;

                    addnote.UploadNoteFile.SaveAs(FileName);

                }


                db.SellerNotesAttachements.Add(sellernotesattachment);
                db.SaveChanges();
                

            }

            return RedirectToAction("addNote");
        }


        [HttpGet]
        public ActionResult searchNotes(int ? i,string search,string type,string category,string country,string university,string course)
        {
            NotesMarketPlaceEntities1 db = new NotesMarketPlaceEntities1();
            ViewData["Type"] = db.NoteTypes.ToList<NoteType>();
            ViewData["Category"] = db.NoteCategories.ToList<NoteCategory>();
            ViewData["Country"] = db.Countries.ToList<Country>();
            ViewData["University"] = db.SellerNotes.Where(e => e.UniversityName != null).Select(e => e.UniversityName).Distinct().ToList();
            ViewData["Course"] = db.SellerNotes.Where(e => e.Course != null).Select(e => e.Course).Distinct().ToList();

            List<SellerNote> notes = db.SellerNotes.Where(e => e.IsActive == true).ToList();

            if (!String.IsNullOrEmpty(search))
            {
                notes = notes.Where(e => e.Title.ToLower().Contains(search.ToLower())).ToList();
            }
         

            if (type != "Select type" && !String.IsNullOrEmpty(type))
            {
                notes = notes.Where(e => e.NoteType != null && e.NoteType1.Name.ToLower().Contains(type.ToLower())).ToList();
               
            }

            if (category != "Select category" && !String.IsNullOrEmpty(category))
            {
                notes = notes.Where(e => e.Category!= null && e.NoteCategory.Name.ToLower().Contains(category.ToLower())).ToList();
            }


            if (country!= "Select country" && !String.IsNullOrEmpty(country))
            {
                notes = notes.Where(e =>e.Country!= null && e.Country1.Name.ToLower().Contains(country.ToLower())).ToList();
            }

            if(university != "Select university" && !String.IsNullOrEmpty(university))
            {
                notes = notes.Where(e => e.UniversityName.ToLower().Contains(university.ToLower())).ToList();
            }

            if (course != "Select course" && !String.IsNullOrEmpty(course))
            {
                notes = notes.Where(e => e.Course.ToLower().Contains(course.ToLower())).ToList();
            }

            ViewBag.Count=notes.Count;
            return View(notes.ToList().ToPagedList(i ?? 1,5));
        }

        [HttpGet]
        public ActionResult BuyerRequests(int? i,string search)
        {
            var userid = (int)Session["ID"];
            var emailid = Session["EmailId"];

            if (emailid != null)
            {
                NotesMarketPlaceEntities1 db = new NotesMarketPlaceEntities1();
                List<Download> BuyerReq = db.Downloads.Where(e=>e.Seller == userid).ToList();

                if (!String.IsNullOrEmpty(search))
                {
                    BuyerReq = BuyerReq.Where(e => e.NoteTitle.ToLower().Contains(search.ToLower())).ToList();
                    if (BuyerReq == null)
                    {
                        BuyerReq = BuyerReq.Where(e => e.NoteCategory.ToLower().Contains(search.ToLower())).ToList();
                    }
                }
                return View(BuyerReq.ToPagedList(i ?? 1, 10));
            }
            else
            {
                return HttpNotFound();
            }               
        }

       
        public ActionResult AllowDownload(int? i,string reqid)
        {
            var userid = (int)Session["ID"];
            var emailid = Session["EmailId"];

            NotesMarketPlaceEntities1 db = new NotesMarketPlaceEntities1();

            List<Download> BuyerReq = db.Downloads.Where(e => e.Seller == userid && e.Downloader.ToString() == reqid).ToList();

            if (reqid != null)
            {
                Download download = db.Downloads.FirstOrDefault(e => e.ID.ToString() == reqid);

                if (download != null)
                {
                    download.IsSellerHasAllowedDownload = true;
                    db.SaveChanges();
                    User user = db.Users.Where(e => e.Id == download.Seller).FirstOrDefault();
                    User downloader = db.Users.Where(e => e.Id == download.Downloader).FirstOrDefault();

                    var verifyUrl = "/RegisteredUsers/BuyerRequests/";
                    var link = "https://localhost:44379/" + verifyUrl;

                    var fromEmail = new MailAddress("notesmarketplacesrs@gmail.com", "Notes MarkePlace");
                    var toEmail = new MailAddress(user.EmailID);
                    var fromEmailPassword = "NotesMP@123"; // Replace with actual password

                    string subject = user.FirstName + " " + user.LastName + " Allows you to download a note";

                    

                    string body = "Hello, "+ downloader.FirstName + " " + downloader.LastName  +" \n\nwe would like to inform you that," + user.FirstName + " " + user.LastName + "  Allows you to download a note. \nplease login and see My Download tabs to download particular note.\n\nRegards,\nNotes Marketplace";


                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
                    };

                    using (var message = new MailMessage(fromEmail, toEmail)
                    {
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true

                    })
                    smtp.Send(message);


                    return RedirectToAction("BuyerRequests","RegisteredUsers");

                }
                else
                {
                    return HttpNotFound();
                }
                
            }

            return RedirectToAction("login", "RegisteredUsers");
        }
              

        [HttpGet]
        public ActionResult FAQ()
        {
            return View();
        }

        [HttpGet]
        public ActionResult changePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult changePassword(ChangePassword changepassword)
        {
            string message = "";

            int uid = Convert.ToInt32(Session["UserId"]);
            using (NotesMarketPlaceEntities1 db = new NotesMarketPlaceEntities1())
            {
                User user = db.Users.Where(m => m.Id == uid).FirstOrDefault();
                
                if (ModelState.IsValid)
                {
                    if (user.Password == changepassword.OldPassword)
                    {
                        user.Password = Crypto.Hash(changepassword.NewPassword);
                        user.ConfirmPassword = Crypto.Hash(changepassword.ConfirmPassword);

                            db.Users.Add(user);
                            db.SaveChanges();
                        
                        message = "Password changed succesfully for " + user.EmailID;
                    }
                    else
                    {
                        message = "Old password is incorrect";
                    }

                }

            }
            ViewBag.Message = message;
            return View();
        }    

        [HttpGet]
        public ActionResult UserProfile()
        {
            NotesMarketPlaceEntities1 db = new NotesMarketPlaceEntities1();
            ViewData["Gender"] = db.ReferenceDatas.ToList<ReferenceData>();
            return View();
        }

        [HttpPost]
        public ActionResult UserProfile(userprofile up)
        {
            using(NotesMarketPlaceEntities1 db = new NotesMarketPlaceEntities1())
            {
                UserProfile uprofile = new UserProfile();

                uprofile.UserID = 2;

                uprofile.DOB = up.DOB;

                uprofile.Gender = up.Gender;

                uprofile.SecondaryEmailAddress = up.SecondaryEmailAddress;

                uprofile.PhoneNumber_CountryCode = up.PhoneNumber_CountryCode;

                uprofile.PhoneNumber = up.PhoneNumber;

                if (up.ProfilePictureFile != null)
                {
                    string FileName = Path.GetFileNameWithoutExtension(up.ProfilePictureFile.FileName);

                    string FileExtension = Path.GetExtension(up.ProfilePictureFile.FileName);

                    FileName = DateTime.Now.ToString("yyyyMMdd") + FileName.Trim() + FileExtension;

                    uprofile.ProfilePicture = FileName;

                    FileName = Path.Combine(Server.MapPath("~/UploadedFiles/") + FileName);

                    up.ProfilePictureFile.SaveAs(FileName);
                }

                uprofile.AddressLine1 = up.AddressLine1;

                uprofile.AddressLine2 = up.AddressLine2;

                uprofile.City = up.City;

                uprofile.State = up.State;

                uprofile.ZipCode = up.ZipCode;

                uprofile.Country = up.Country;

                uprofile.University = up.University;

                uprofile.College = up.College;

                uprofile.CreatedDate = DateTime.Now;

                db.UserProfiles.Add(uprofile);
                db.SaveChanges();

            }

            return RedirectToAction("Dashboard");
        }  
    

        [HttpGet]
        public ActionResult NoteDetails(string noteid)
        {
            using(NotesMarketPlaceEntities1 db = new NotesMarketPlaceEntities1())
            {
                SellerNote sn = db.SellerNotes.FirstOrDefault(m => m.ID.ToString().Equals(noteid));
                if(sn!=null)
                {
                    bool isvalid = db.Downloads.Any(m => m.NoteID.ToString().Equals(noteid) && m.Seller == temp && m.IsSellerHasAllowedDownload);
                    if(isvalid)
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
            using(NotesMarketPlaceEntities1 db = new NotesMarketPlaceEntities1())
            {
                bool isexist = db.Downloads.Any(m => m.NoteID.ToString().Equals(noteid) && m.Seller == temp);

                var note = db.SellerNotes.FirstOrDefault(m => m.ID.ToString().Equals(noteid));

                if (!isexist)
                {
                    Download download = new Download();
                    download.NoteID = Convert.ToInt32(noteid);
                    download.Seller = temp;
                    download.NoteTitle = note.Title;
                    download.PurchasedPrice = note.SellingPrice;
                    download.Downloader = temp;
                    download.IsSellerHasAllowedDownload = true;
                    download.IsAttachedDownloaded = true;
                    download.NoteCategory = note.NoteCategory.Name;
                    download.CreatedDate = DateTime.Now;
                    db.Downloads.Add(download);
                    db.SaveChanges();
                }
                var path = note.SellerNotesAttachements.FirstOrDefault(m => m.NoteID.ToString().Equals(noteid)).FilePath;
                return RedirectToAction("Download", "RegisteredUsers", new {path=path});
            }
            return RedirectToAction("NoteDetails",noteid);
        }

        public ActionResult PaidNotes(string noteid)
        {
            using (NotesMarketPlaceEntities1 db = new NotesMarketPlaceEntities1())
            {
                bool isexist = db.Downloads.Any(m => m.NoteID.ToString().Equals(noteid) && m.Seller == temp);
                bool isvalid = db.Downloads.Any(m => m.NoteID.ToString().Equals(noteid) && m.Seller == temp && m.IsSellerHasAllowedDownload);
                var note = db.SellerNotes.FirstOrDefault(m => m.ID.ToString().Equals(noteid));

                if (!isexist)
                {
                    Download download = new Download();
                    download.NoteID = Convert.ToInt32(noteid);
                    download.Seller = temp;
                    download.NoteTitle = note.Title;
                    download.PurchasedPrice = note.SellingPrice;
                    download.Downloader = temp;
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

        public ActionResult MyDownloads()
        {
            return View();
        }



    }

}