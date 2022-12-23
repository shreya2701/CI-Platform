using CI_Platform_Project.DataModels;
using CI_Platform_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Net.Mail;

namespace CI_Platform_Project.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        public CiPlatformContext _db = new CiPlatformContext();
        public List<Mission> temp;
        public IActionResult Index()
        {
            HomePage homePage = new HomePage();
            var carddata = new List<Card>();
            temp = _db.Missions.Where(x => x.DeletedAt == null).AsEnumerable().ToList();
            foreach (var item in temp)
            {
                var card = new Card();
                card.mission = item;

                card.theme = _db.MissionThemes.FirstOrDefault(x => x.MissionThemeId == item.ThemeId).Title;
                card.city = _db.Cities.FirstOrDefault(x => x.CityId == item.CityId).Name;
                var image = _db.MissionMedia.FirstOrDefault(x => x.MissionId == item.MissionId);
                card.img = image.MediaPath;
                var fav = _db.FavoriteMissions.FirstOrDefault(x => x.MissionId == item.MissionId && x.UserId == int.Parse(HttpContext.Session.GetString("UserId")) && x.DeletedAt == null);
                card.favorite = fav != null ? 1 : 0;
                card.GoalMission = _db.GoalMissions.FirstOrDefault(x => x.MissionId == item.MissionId);
                card.seatleft = (int)(item.TotalSeat - (_db.MissionApplications.Where(x => x.MissionId == item.MissionId).Count()));
                carddata.Add(card);
            }
            homePage.card = carddata;


           // IEnumerable<SelectListItem> ThemeList = _db.MissionThemes.Where(x => x.DeletedAt == null).Select(
           //         u => new SelectListItem
           //         {
           //             Text = u.Title,
           //             Value = u.MissionThemeId.ToString()
           //         }
           //     );
           // IEnumerable<SelectListItem> skilllist = _db.Skills.Where(x => x.DeletedAt == null).Select(
           //     u => new SelectListItem
           //     {
           //         Text = u.SkillName,
           //         Value = u.SkillId.ToString()
           //     }
           // );
           // IEnumerable<SelectListItem> CityList = _db.Cities.Where(x => x.DeletedAt == null).Select(
           //    u => new SelectListItem
           //    {
           //        Text = u.Name,
           //        Value = u.CityId.ToString()
           //    }
           //);
           // IEnumerable<SelectListItem> CountryList = _db.Countries.Where(x => x.DeletedAt == null).Select(
           //     u => new SelectListItem
           //     {
           //         Text = u.Name,
           //         Value = u.CountryId.ToString()
           //     }
           // );


           // ViewBag.ThemeList = ThemeList;
           // ViewBag.CityList = CityList;
           // ViewBag.skilllist = skilllist;
           // ViewBag.CountryList = CountryList;

            return View(homePage);
        }

        [HttpGet]
        public IActionResult CoWorker(int id)
        {
            var mission = new Mission();
            mission.MissionId = id;
            return PartialView("_Co-WorkerPartial", mission);
        }

        [HttpPost]
        public JsonResult CoWorker(string InviteEmail, Mission obj)
        {
            var mission = _db.Missions.FirstOrDefault(x => x.MissionId == obj.MissionId);
            string message = "";
            bool status = false;
            string errorMessage = "Eneter Valid Email";
            //string InviteEmial = null;
            string baseUrl = string.Format("{0}://{1}", HttpContext.Request.Scheme, HttpContext.Request.Host);
            var activationUrl = $"{baseUrl}/Customer/User/Login";
            //var link = Request.URL.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("shreyakanani46@gmail.com", "Shreya Tatvasoft");
            var toEmail = new MailAddress(InviteEmail);


            var fromEmailPassword = "gndthjyluwlsuuma"; // Replace with actual password


            string subject = "";
            string body = "";


            subject = "Invite For Mission Apply";
            //body = "Hi,<br/>br/>We got request for reset your account password. Please click on the below link to reset your password" +
            //    "<br/><br/><a href='" + activationUrl + "'>Reset Password link</a>";
            body = "<h1>" + "Shreya" + " Suggest Mission : " + mission.Title + " to You</h1><br><h2><a href='https://localhost:7050'>Go Website</a></h2>";
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
            return Json(obj);

        }


        [HttpPost]
        public JsonResult favoriteMission(int id, int x)
        {
            if (x == 1)
            {
                var favorite = new FavoriteMission();
                var mission = new MissionZm();
                favorite.MissionId = id;
                favorite.UserId = int.Parse(HttpContext.Session.GetString("UserId"));
                _db.FavoriteMissions.Add(favorite);
                _db.SaveChanges();
            }
            else
            {
                var favorite = _db.FavoriteMissions.FirstOrDefault(x => x.MissionId == id && x.UserId == int.Parse(HttpContext.Session.GetString("UserId")) && x.DeletedAt == null);
                favorite.DeletedAt = DateTime.Now;
                _db.FavoriteMissions.Update(favorite);
                _db.SaveChanges();
            }
            return Json(true);
        }

        [HttpPost]
        public IActionResult Filter(int? sortOrder, string searchString)
        {
            var cardDisplay = new List<Card>();
            HomePage homePage = new HomePage();
            var filterMission = new List<Index>();
            if (!String.IsNullOrEmpty(searchString))
            {
                var mission1 = _db.Missions.Where(s => s.Title.Contains(searchString) && s.DeletedAt == null);

                foreach (var item in mission1)
                {  
                    var x = _db.Missions.Where(x => x.MissionId == item.MissionId);
                    var card = new Card();
                    card.mission = item;
                    card.city = _db.Cities.Where(x => x.CityId == item.CityId).FirstOrDefault().Name;
                    card.theme = _db.MissionThemes.FirstOrDefault(x => x.MissionThemeId == item.ThemeId).Title;
                   
                    var image = _db.MissionMedia.FirstOrDefault(x => x.MissionId == item.MissionId);
                    card.img = image.MediaPath;
                    var fav = _db.FavoriteMissions.FirstOrDefault(x => x.MissionId == item.MissionId && x.UserId == int.Parse(HttpContext.Session.GetString("UserId")) && x.DeletedAt == null);
                    card.favorite = fav != null ? 1 : 0;
                    card.GoalMission = _db.GoalMissions.FirstOrDefault(x => x.MissionId == item.MissionId);
                    card.seatleft = (int)(item.TotalSeat - (_db.MissionApplications.Where(x => x.MissionId == item.MissionId).Count()));
                    cardDisplay.Add(card);
                }
                homePage.card = cardDisplay;
                return View(homePage);
            }

            switch (sortOrder)
            {
                case 1:
                    cardDisplay = cardDisplay.OrderByDescending(x => x.mission.CreatedAt).ToList();
                    break;
                case 2:
                    cardDisplay = cardDisplay.OrderBy(x => x.mission.CreatedAt).ToList();
                    break;
                case 3:
                    cardDisplay = cardDisplay.OrderBy(x => x.seatleft).ToList();
                    break;
                case 4:
                    cardDisplay = cardDisplay.OrderByDescending(x => x.seatleft).ToList();
                    break;
                case 5:
                    cardDisplay = cardDisplay.OrderByDescending(x => x.favorite).ToList();
                    break;
                case 6:
                    cardDisplay = cardDisplay.OrderByDescending(x => x.mission.RegistrationDeadline).ToList();
                    break;
            }

            return PartialView("_MissionCardPartial", cardDisplay);
        }
    }
}
