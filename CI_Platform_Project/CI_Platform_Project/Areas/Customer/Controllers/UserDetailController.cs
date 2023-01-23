using CI_Platform_Project.DataModels;
using CI_Platform_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Net.Mail;

namespace CI_Platform_Project.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class UserDetailController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        public UserDetailController(IWebHostEnvironment hostingEnvironment)
        {

            _hostEnvironment = hostingEnvironment;
        }

        public CiPlatformContext _db = new CiPlatformContext();
        public IActionResult UserProfile()
        {
            var user = new UserDetail();
            var id = int.Parse(HttpContext.Session.GetString("UserId"));
            user.User = _db.Users.FirstOrDefault(x => x.UserId == id);
            user.UserSkill = _db.UserSkills.Where(x => x.UserId == id && x.DeletedAt == null).Select(x => x.Skill.SkillName).ToList();  
            user.User.Avatar =  user.User.Avatar != null ? user.User.Avatar : "~/lib/assets/volunteer1.png";
            IEnumerable<SelectListItem> CityList = _db.Cities.Where(x => x.DeletedAt == null).Select(
               u => new SelectListItem
               {
                   Text = u.Name,
                   Value = u.CityId.ToString()
               }
           );
            IEnumerable<SelectListItem> CountryList = _db.Countries.Where(x => x.DeletedAt == null).Select(
                u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.CountryId.ToString()
                }
            );
            IEnumerable<SelectListItem> SkillList = _db.Skills.Where(x => x.DeletedAt == null).Select(
                u => new SelectListItem
                {
                    Text = u.SkillName,
                    Value = u.SkillId.ToString()
                }
            );
            ViewBag.CityList = CityList;
            ViewBag.CountryList = CountryList;
            ViewBag.SkillList = SkillList;
            return View(user);
        }
        [HttpPost]
        public IActionResult UserProfile(UserDetail obj ,IFormFile file)
        {
            obj.User.UpdatedAt = DateTime.Now;
            if (file != null)
            {
                obj.User.Avatar = img(file);
            }
            _db.Users.Update(obj.User);
            _db.SaveChanges();
            return RedirectToAction("Index", "Home", new { areas = "Customer" });
        }

        public string img(IFormFile file)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;

            string fileName = Guid.NewGuid().ToString();
            var Uploads = Path.Combine(wwwRootPath, @"images\StoryImage");
            var extension = Path.GetExtension(file.FileName);

            //if (missionMedia.MediaName != null)
            //{
            //    var oldImagePath = Path.Combine(wwwRootPath, missionMedia.MediaName.TrimStart('\\'));
            //    if (System.IO.File.Exists(oldImagePath))
            //    {
            //        System.IO.File.Delete(oldImagePath);
            //    }
            //}

            using (var fileSrteam = new FileStream(Path.Combine(Uploads, fileName + extension), FileMode.Create))
            {
                file.CopyTo(fileSrteam);
            }
            return @"\images\StoryImage\" + fileName + extension;
        }

        [HttpGet]
        public IActionResult ConfirmPassord()
        {
            var obj = new UserDetail();
            var id = int.Parse(HttpContext.Session.GetString("UserId"));
            obj.User = _db.Users.FirstOrDefault(x => x.UserId == id);
            return PartialView("_ConfirmPasswordPartial", obj);
        }

        [HttpPost]
        public IActionResult ConfirmPassord(UserDetail obj)
        {
            
            var id = int.Parse(HttpContext.Session.GetString("UserId"));
            obj.User = _db.Users.FirstOrDefault(x => x.UserId == id);
            if(obj.User.Password == obj.Password)
            {
                obj.User.Password = obj.newPassword;
                _db.Users.Update(obj.User);
                _db.SaveChanges();
                return RedirectToAction("UserProfile", "UserDetail", new { areas = "Customer" });
            }
            else
            {
                TempData["errorMessage"] = "Email and Password Is Incorrect";
                
            }
            return PartialView("_ConfirmPasswordPartial", obj);
        }


        [HttpGet]
        public IActionResult ContactUs()
        {
            var obj = new ContactUs();
            var id = int.Parse(HttpContext.Session.GetString("UserId"));
            obj.UserId = id;
            return PartialView("_ContactUsPartial", obj);
        }
        [HttpPost]
        public IActionResult ContactUs(ContactUs obj)
        {
            obj.UserId = int.Parse(HttpContext.Session.GetString("UserId"));
            var user = _db.Users.FirstOrDefault(x => x.UserId == obj.UserId);
            obj.Name = user.FirstName + " " + user.LastName;
            obj.Email = user.Email;
            string message = "";
            bool status = false;
            string errorMessage = "Eneter Valid Email";
            //string InviteEmial = null;
            string baseUrl = string.Format("{0}://{1}", HttpContext.Request.Scheme, HttpContext.Request.Host);
            var activationUrl = $"{baseUrl}/Customer/User/Login";
            //var link = Request.URL.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("shreyakanani46@gmail.com", "Shreya Tatvasoft");
            var toEmail = new MailAddress("shreyakanani46@gmail.com");


            var fromEmailPassword = "gndthjyluwlsuuma"; // Replace with actual password


            string subject = "";
            string body = "";


            subject = "Invite For Mission Apply";
            //body = "Hi,<br/>br/>We got request for reset your account password. Please click on the below link to reset your password" +
            //    "<br/><br/><a href='" + activationUrl + "'>Reset Password link</a>";
            body ="  User Id  = " + obj.UserId + " </ br > User Name  = " + obj.Name + " </ br >  User Email  = " + obj.Email + " </ br >  Subject  = " + obj.Subject + " </ br >  message  = " + obj.Message;
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
            return RedirectToAction("UserProfile", "UserDetail" , new { areas = "Customer" });

        }

        public IActionResult Privacy()
        {
            var privacy = _db.CmsPages.Where(x => x.DeletedAt == null).ToList();
            return View(privacy);
        }
    }
}
