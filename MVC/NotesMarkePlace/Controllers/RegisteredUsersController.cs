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

namespace NotesMarkePlace.Controllers
{

    public class RegisteredUsersController : Controller
    {

        // GET: Login
        [HttpGet]
        public ActionResult login()
        {
            return View();
        }

        // POST : Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult login(User user, string ReturnUrl = "")
        {
            string message = "";
            using (NotesMarketPlaceEntities1 db = new NotesMarketPlaceEntities1())
            {
                var v = db.Users.Where(a => a.EmailID == user.EmailID).FirstOrDefault();
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


                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("index", new RouteValueDictionary(new { Controller = "RegisteredUsers", Action = "index" }));
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
        public ActionResult index()
        {
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
            return View();
        }

        [HttpPost]
        public ActionResult addnote(SellerNote sellernote, HttpPostedFileBase file)
        {
            bool Status = false;
            string message = "";

            // Model Validation
            if (ModelState.IsValid)
            {
                #region
                // Save To Database
                using (NotesMarketPlaceEntities1 db = new NotesMarketPlaceEntities1())
                {
                    try
                    {
                        if (file.ContentLength > 0)
                        {
                            string _FileName = Path.GetFileName(file.FileName);
                            string _path = Path.Combine(Server.MapPath("~/UploadedFiles"), _FileName);
                            file.SaveAs(_path);
                        }
                        ViewBag.Message = "File Uploaded Successfully!!";
                        return View();
                    }
                    catch
                    {
                        ViewBag.Message = "File upload failed!!";
                        return View();
                    }

                    db.SellerNotes.Add(sellernote);
                    db.SaveChanges();

                    message = "Notes Added Sucessfully";
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
            return View(sellernote);
        }

        [HttpGet]
        public ActionResult searchNotes()
        {
            return View();
        }

        [HttpGet]
        public ActionResult BuyerRequests()
        {
            return View();
        }
    }
}