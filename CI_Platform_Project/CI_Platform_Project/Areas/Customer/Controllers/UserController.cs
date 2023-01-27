using CI_Platform_Project.DataModels;
using CI_Platform_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using static System.Net.WebRequestMethods;

namespace CI_Platform_Project.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class UserController : Controller
    {

        //Login
        //get
        User user = new User();
        PasswordReset passwordReset = new PasswordReset();
        public CiPlatformContext _db = new CiPlatformContext();

        [AllowAnonymous]
        public IActionResult Login()
        {
            Login login = new Login();
            login.banner = GetBanner();
            return View(login);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(Login model)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email.Equals(model.User.Email.ToLower()) && u.Password.Equals(model.User.Password) && u.DeletedAt == null && u.Status == 1);
            
            if (user == null)
            {
                var admin = _db.Admins.FirstOrDefault(u => u.Email.Equals(model.User.Email.ToLower()) && u.Password.Equals(model.User.Password));
                if (admin != null)
                {
                    HttpContext.Session.SetString("UserId", admin.AdminId.ToString());
                    HttpContext.Session.SetString("UserName", admin.FirstName.ToString());
                    return RedirectToAction("Index", "Admin", new { area = "Admin" });
                }

                Login login = new Login();
                login.banner = GetBanner();
                TempData["errorMessage"] = "Email And Password Incorrect";
                return View(login);

            }
            if(user.Status != 1)
            {
                Login login = new Login();
                login.banner = GetBanner();
                TempData["errorMessage"] = "You are no longer User.";
                return View(login);

            }
           
            HttpContext.Session.SetString("UserId", user.UserId.ToString());
            HttpContext.Session.SetString("UserName", user.FirstName.ToString());
            return RedirectToAction("Index", "Home", new { area = "Customer" });
            
        }
        //Login End

        public List<Banner> GetBanner()
        {
            return _db.Banners.Where(x => x.DeletedAt == null).ToList();
        }

        //Registration 
        //get
        public async Task<IActionResult> Registration()
        {
            Login login = new Login();
            login.banner = GetBanner();
            return View(login);
        }


        //post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registration(Login model)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email.Equals(model.User.Email.ToLower()) && u.DeletedAt == null);

            if (user == null)
            {
                _db.Users.Add(model.User);
                _db.SaveChanges();
                return RedirectToAction("Login", "User");
            }
            TempData["ErrorMes"] = "User alraedy exist with same email";
            Login login = new Login();
            login.banner = GetBanner();
            return View(login);
        }

        //Registration End



        //Forget Start

        public IActionResult ForgotPassword()
        {
            Login login = new Login();
            login.banner = GetBanner();
            return View(login);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ForgotPassword(Login model)
        {
            string message = "";
            bool status = false;

            using (CiPlatformContext _db = new CiPlatformContext())
            {
                var account = _db.Users.Where(u => u.Email.Equals(model.User.Email)).FirstOrDefault();
                
                    
                
                var passwordReset = new PasswordReset();
                // var passwordReset = _db.PasswordResets.Where(u => u.Email.Equals(model.Email)).FirstOrDefault();
                if (account != null)
                {

                    string token = Guid.NewGuid().ToString();
                    passwordReset.Token = token;
                    //send email for reset password
                    var y = token;
                    //var verifyUrl = "/User/" + emailFor + "/" + activationCode;
                    string baseUrl = string.Format("{0}://{1}", HttpContext.Request.Scheme, HttpContext.Request.Host);
                    var activationUrl = $"{baseUrl}/Customer/User/ResetPassword?code={y}";
                    //var link = Request.URL.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

                    var fromEmail = new MailAddress("shreyakanani46@gmail.com", "Dotnet Awesome");
                    var toEmail = new MailAddress(model.User.Email);
                    var fromEmailPassword = "gndthjyluwlsuuma"; // Replace with actual password


                    string subject = "";
                    string body = "";


                    subject = "Reset Password";
                    body = "Hi,<br/>br/>We got request for reset your account password. Please click on the below link to reset your password" +
                        "<br/><br/><a href='" + activationUrl + "'>Reset Password link</a>";

                    // < a href = '7050/User/ResetPassword' >
                    ServicePointManager.Expect100Continue = false;

                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)

                    };

                    var x = new MailMessage(fromEmail, toEmail)
                    {
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };
                    smtp.Send(x);

                    // SendVerificationLinkEmail(account.Email, token, "ResetPassword");
                    passwordReset.Token = token;
                    passwordReset.Email = model.User.Email;
                    _db.PasswordResets.Add(passwordReset);
                    //This line I have added here to avoid confirm password not match issue , as we had added a confirm password property 
                    //in our model class in part 1
                    //_db.Configuration.ValidateOnSaveEnabled = false;
                    _db.SaveChanges();
                    
                }
                else
                {
                    Login login = new Login();
                    login.banner = GetBanner();
                    TempData["errorMessage"] = "Incoorect Mail.Plz enter correct mail";
                    return View(login);
                }
            }
            TempData["errorMessage"] = "Mail is Send. Check Your Mail";
            return RedirectToAction("Login", "User");
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }




        //Reset Paasword
        public ActionResult ResetPassword()
        {
            //Verify the reset password link
            //Find account associated with this link
            //redirect to reset password page
            //if (string.IsNullOrWhiteSpace(id))
            //{
            //    return HttpNotFound();
            //}
            Login login = new Login();
            login.banner = GetBanner();
            return View(login);

        }


        private ActionResult HttpNotFound()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(User model, string code)
        {

            var resetUser = _db.PasswordResets.OrderByDescending(x => x.CreatedAt).FirstOrDefault(x => x.Token.Equals(code));
            var user = new User();
            if (resetUser != null)
            {
                user = _db.Users.FirstOrDefault(x => x.Email.Equals(resetUser.Email));

                user.Password = model.Password;
                _db.Users.Update(user);
                _db.SaveChanges();
                return RedirectToAction("Login", "User");
            }

            return RedirectToAction("Register", "User");
        }


        public ActionResult Banner()
        {
            return View();
        }
    }
}

