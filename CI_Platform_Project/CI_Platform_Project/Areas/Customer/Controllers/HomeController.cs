using CI_Platform_Project.DataModels;
using CI_Platform_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Net;
using System.Net.Mail;
using PagedList;

namespace CI_Platform_Project.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        public HomeController(IWebHostEnvironment hostingEnvironment)
        {

            _hostEnvironment = hostingEnvironment;
        }
        public CiPlatformContext _db = new CiPlatformContext();
        HomePage homePage = new HomePage();
        public IActionResult Index(int pg = 1)
        {
            HomePage homePage = new HomePage();
            var carddata = new List<Card>();
            var temp = _db.Missions.Where(x => x.DeletedAt == null && x.Status == 1).AsEnumerable().ToList();

            foreach (var item in temp)
            {
                carddata.Add(MissionCard(item));
            }
            //homePage.card = carddata;

            const int pageSize = 9;

            int recsCount = temp.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = carddata.Skip(recSkip).Take(pager.PageSize).ToList();
            homePage.card = data;
            this.ViewBag.Pager = pager;

            IEnumerable<SelectListItem> ThemeList = _db.MissionThemes.Where(x => x.DeletedAt == null).Select(
                    u => new SelectListItem
                    {
                        Text = u.Title,
                        Value = u.MissionThemeId.ToString()
                    }
                );
            IEnumerable<SelectListItem> skilllist = _db.Skills.Where(x => x.DeletedAt == null).Select(
                u => new SelectListItem
                {
                    Text = u.SkillName,
                    Value = u.SkillId.ToString()
                }
            );
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


            ViewBag.ThemeList = ThemeList;
            ViewBag.CityList = CityList;
            ViewBag.skilllist = skilllist;
            ViewBag.CountryList = CountryList;

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
        public IActionResult favoriteMission(int id, int x)
        {
            var carddata = new List<Card>();
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

            var temp = _db.Missions.Where(x => x.DeletedAt == null && x.Status == 1).AsEnumerable().ToList();
            foreach (var item in temp)
            {
                carddata.Add(MissionCard(item));
            }
            homePage.card = carddata;
            return PartialView("_MissionCardPartial", carddata);
        }

        public JsonResult ApplyMission(int id)
        {
            var missionApp = new MissionApplication();
            if (_db.MissionApplications.FirstOrDefault(x => x.MissionId == id && x.UserId == int.Parse(HttpContext.Session.GetString("UserId")) && x.ApprovalStatus == 1) != null)
            {
                return Json("You are part of Mission");
            }
            else if (_db.MissionApplications.FirstOrDefault(x => x.MissionId == id && x.UserId == int.Parse(HttpContext.Session.GetString("UserId"))) != null)
            {
                return Json("You already applied");
            }
            else
            {
                missionApp.MissionId = id;
                missionApp.ApprovalStatus = 0;
                missionApp.UserId = int.Parse(HttpContext.Session.GetString("UserId"));
                missionApp.AppliedAt = DateTime.Now;
                _db.MissionApplications.Add(missionApp);
                _db.SaveChanges();
                return Json("applied Successfully");
            }
        }

        [HttpPost]
        public IActionResult Filter(int? sortOrder, string searchString, List<int> cityFilter, List<int> countryFilter, List<int> themeFilter, List<int> skillFilter, string currentFilter, int? pageNumber, int pg = 1)
        {
            var cardDisplay = new List<Card>();
            var missionList = new List<Mission>();
            ViewData["CurrentSort"] = sortOrder;

            if (!String.IsNullOrEmpty(searchString))
            {
                var mission1 = _db.Missions.Where(s => s.Title.Contains(searchString) && s.DeletedAt == null).AsEnumerable().ToList();

                foreach (var item in mission1)
                {
                    cardDisplay.Add(MissionCard(item));
                }
                homePage.card = cardDisplay;

            }
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            if (cityFilter != null)
            {
                foreach (var obj in cityFilter)
                {
                    var mission1 = _db.Missions.Where(x => x.DeletedAt == null && x.Status == 1 && x.CityId == obj).AsEnumerable().ToList();

                    foreach (var item in mission1)
                    {
                        if (!cardDisplay.Any(x => x.mission.MissionId == item.MissionId))
                        {
                            cardDisplay.Add(MissionCard(item));
                        }
                    }
                }
                homePage.card = cardDisplay;
            }

            if (countryFilter != null)
            {
                foreach (var obj in countryFilter)
                {
                    var mission1 = _db.Missions.Where(x => x.DeletedAt == null && x.Status == 1 && x.CountryId == obj).AsEnumerable().ToList();

                    foreach (var item in mission1)
                    {
                        if (!cardDisplay.Any(x => x.mission.MissionId == item.MissionId))
                        {
                            cardDisplay.Add(MissionCard(item));
                        }
                    }
                }
                homePage.card = cardDisplay;
            }

            if (themeFilter != null)
            {
                foreach (var obj in themeFilter)
                {
                    var mission1 = _db.Missions.Where(x => x.DeletedAt == null && x.Status == 1 && x.ThemeId == obj).AsEnumerable().ToList();

                    foreach (var item in mission1)
                    {
                        if (!cardDisplay.Any(x => x.mission.MissionId == item.MissionId))
                        {
                            cardDisplay.Add(MissionCard(item));
                        }

                    }
                }
                homePage.card = cardDisplay;
            }

            if (skillFilter != null)
            {
                foreach (var obj in skillFilter)
                {
                    var missions = new List<Mission>();
                    var mission1 = _db.MissionSkills.Where(x => x.SkillId == obj && x.DeletedAt == null).Select(m => m.MissionId).AsEnumerable().ToList();
                    foreach (var mission in mission1)
                    {
                        missions.Add(_db.Missions.FirstOrDefault(x => x.MissionId == mission && x.DeletedAt == null));
                    }
                    foreach (var v in missions)
                    {
                        if (!cardDisplay.Any(x => x.mission.MissionId == v.MissionId))
                        {
                            cardDisplay.Add(MissionCard(v));
                        }
                        else
                        {

                        }
                    }
                }
                homePage.card = cardDisplay;
            }

            if (searchString == null && themeFilter.Count == 0 && cityFilter.Count == 0 && countryFilter.Count == 0 && skillFilter.Count == 0)
            {
                var mission1 = _db.Missions.Where(x => x.DeletedAt == null && x.Status == 1).AsEnumerable().ToList();
                foreach (var item in mission1)
                {
                    cardDisplay.Add(MissionCard(item));
                }
            }

            if (sortOrder != null)
            {
                var temp = _db.Missions.Where(x => x.DeletedAt == null && x.Status == 1).AsEnumerable().ToList();
                foreach (var v in temp)
                {
                    if (!cardDisplay.Any(x => x.mission.MissionId == v.MissionId))
                    {
                        cardDisplay.Add(MissionCard(v));
                    }
                }
                homePage.card = cardDisplay;
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
                        cardDisplay = cardDisplay.Where(x => x.favorite == 1).ToList();
                        break;
                    case 6:
                        cardDisplay = cardDisplay.OrderByDescending(x => x.mission.RegistrationDeadline).ToList();
                        break;
                }
            }
            const int pageSize = 9;

            int recsCount = missionList.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = cardDisplay.Skip(recSkip).Take(pager.PageSize).ToList();
            homePage.card = data;
            this.ViewBag.Pager = pager;



            return PartialView("_MissionCardPartial", cardDisplay);
        }

        public IActionResult logOut()
        {
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Remove("UserName");
            return RedirectToAction("Login", "User", new { areas = "Customer" });
        }

        public IActionResult missionDetail(int id)
        {
            var missionDetail = new MissionDetail();
            var item = _db.Missions.FirstOrDefault(x => x.MissionId == id);
            missionDetail.missionCard = MissionCard(item);
            missionDetail.missionImages = _db.MissionMedia.Where(x => x.MissionId == id && x.DeletedAt == null).ToList();
            //var x =  _db.MissionSkills.Where(x => x.MissionId == id && x.DeletedAt == null);
            //missionDetail.skills = x.Select(y => y.Skill.SkillName).ToList() ;
            missionDetail.skills = String.Join(", ", _db.MissionSkills.Where(x => x.MissionId == id && x.DeletedAt == null).Select(x => x.Skill.SkillName).ToList());
            var missionApp = _db.MissionApplications.Where(x => x.MissionId == id && x.ApprovalStatus == 1).AsEnumerable().ToList();
            var temp = _db.MissionRatings.FirstOrDefault(x => x.MissionId == id && x.UserId == int.Parse(HttpContext.Session.GetString("UserId")));
            if (temp != null)
            {
                missionDetail.Rating = temp.Rating;
            }
            else
            {
                missionDetail.Rating = 0;   
            }
            var listVol = new List<RecentVolunteer>();
            foreach (var u in missionApp)
            {
                var volunteer = new RecentVolunteer();
                var user = _db.Users.FirstOrDefault(x => x.UserId == u.UserId);
                volunteer.VolunteerName = user.FirstName + " " + user.LastName;
                volunteer.VolunteerImg = user.Avatar == null ? "~/lib/assets/volunteer1.png" : user.Avatar;
                listVol.Add(volunteer);
            }
            missionDetail.recentVolunteers = listVol;
            missionDetail.relatedMission = RelatedMission((int)item.CityId, (int)item.CountryId, (int)item.ThemeId);

            return View(missionDetail);
        }

        public string RateMission(long Mid, int Rating)
        {
            var temp = _db.MissionRatings.FirstOrDefault(u => u.MissionId == Mid && u.UserId == int.Parse(HttpContext.Session.GetString("UserId")));
            if (temp != null)
            {
                temp.Rating = Rating;
                temp.UpdatedAt = DateTime.Now;
                _db.MissionRatings.Update(temp);
                _db.SaveChanges();
            }
            else
            {
                MissionRating missionRating = new MissionRating();
                missionRating.MissionId = Mid;
                missionRating.Rating = Rating;
                missionRating.UserId = int.Parse(HttpContext.Session.GetString("UserId"));
                missionRating.CreatedAt = DateTime.Now;
                _db.MissionRatings.Add(missionRating);
                _db.SaveChanges();
            }
            return ("Done");
            }


        public Card MissionCard(Mission item)
        {
            var card = new Card();
            card.mission = item;
            card.theme = _db.MissionThemes.FirstOrDefault(x => x.MissionThemeId == item.ThemeId && x.DeletedAt == null && x.Status == 1).Title;
            var img = _db.MissionMedia.FirstOrDefault(x => x.MissionId == item.MissionId);
            card.img = img != null ? img.MediaPath : "~/lib/assets/card-1.png";
            card.city = _db.Cities.FirstOrDefault(x => x.CityId == item.CityId && x.DeletedAt == null).Name;
            card.seatleft = (int)(item.TotalSeat - (_db.MissionApplications.Where(x => x.MissionId == item.MissionId).Count()));
            card.GoalMission = _db.GoalMissions.FirstOrDefault(x => x.MissionId == item.MissionId);
            card.favorite = _db.FavoriteMissions.FirstOrDefault(x => x.MissionId == item.MissionId && x.UserId == int.Parse(HttpContext.Session.GetString("UserId")) && x.DeletedAt == null) != null ? 1 : 0;


            return card;
        }




        public IActionResult Story(int pg = 1)
        {
            var story = new StoryZm();
            var StoryCard = new List<StoryCard>();
            var temp = _db.Stories.Where(x => x.DeletedAt == null && x.Status == 1).AsEnumerable().ToList();
            foreach (var item in temp)
            {
                var card = new StoryCard();
                card.Stories = item;
                var img = _db.MissionMedia.FirstOrDefault(x => x.MissionId == item.MissionId);
                card.img = img != null ? img.MediaPath : "~/lib/assets/card-1.png";
                var missionTheme = _db.Missions.FirstOrDefault(x => x.MissionId == item.MissionId);
                card.theme = _db.MissionThemes.FirstOrDefault(x => x.MissionThemeId == missionTheme.ThemeId && x.DeletedAt == null).Title;
                var user = _db.Users.FirstOrDefault(x => x.UserId == item.UserId);
                card.UserName = user.LastName + " " + user.FirstName;
                card.UserImg = user.Avatar != null ? user.Avatar : "~/lib/assets/volunteer1.png";

                StoryCard.Add(card);
            }
            //story.storyCard = StoryCard;
            const int pageSize = 3;

            int recsCount = temp.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = StoryCard.Skip(recSkip).Take(pager.PageSize).ToList();
            story.storyCard = data;
            this.ViewBag.Pager = pager;

            return View(story);
        }

        public IActionResult ShareStory()
        {
            IEnumerable<SelectListItem> StoryList = _db.Missions.Where(x => x.DeletedAt == null).Select(
                    u => new SelectListItem
                    {
                        Text = u.Title,
                        Value = u.MissionId.ToString()
                    }
                );
            ViewBag.StoryList = StoryList;
            return View();
        }
        [HttpPost]
        public IActionResult ShareStory(StoryZm obj, List<IFormFile> file, string VideoUrl)
        {

            obj.story.UserId = int.Parse(HttpContext.Session.GetString("UserId"));
            _db.Stories.Add(obj.story);
            _db.SaveChanges();


            MissionMedium missionMedium = new MissionMedium();
            string wwwRootPath = _hostEnvironment.WebRootPath;
            if (file != null)
            {
                foreach (var x in file)
                {
                    var storyedium = new StoryMedium();
                    storyedium.StoryId = obj.story.StoryId;
                    storyedium.Type = Path.GetExtension(x.FileName);
                    storyedium.Path = img(x);

                    _db.StoryMedia.Add(storyedium);
                    _db.SaveChanges();
                }
            }


            var storyedium1 = new StoryMedium();
            storyedium1.StoryId = obj.story.StoryId;
            storyedium1.Path = VideoUrl;
            _db.StoryMedia.Add(storyedium1);
            _db.SaveChanges();

            return RedirectToAction("Story", "Home", new { Areas = "Customer" });
        }


        public IActionResult StoryDetail(int id)
        {
            var storyDetail = new StoryDetail();
            var story = _db.Stories.FirstOrDefault(x => x.StoryId == id);
            //story.view_count += 1;
            _db.Stories.Update(story);
            _db.SaveChanges();
            var obj = _db.Stories.FirstOrDefault(x => x.StoryId == id);
            storyDetail.storyId = obj.StoryId;
            storyDetail.viewCount += 1;
            storyDetail.missionId = obj.MissionId;
            storyDetail.storyTitle = obj.Title;
            storyDetail.storyDescription = obj.Description;
            var x = _db.StoryMedia.Where(x => x.StoryId == id);
            storyDetail.storyImg = x.Select(x => x.Path).ToList();
            storyDetail.storyMedium = _db.StoryMedia.Where(x => x.StoryId == id && x.DeletedAt == null).ToList();
            var y = _db.Users.FirstOrDefault(x => x.UserId == obj.UserId);
            storyDetail.userName = y.FirstName + " " + y.LastName;
            storyDetail.userImg = y.Avatar != null ? y.Avatar : "~/lib/assets/volunteer1.png";
            return View(storyDetail);
        }


        public List<Card> RelatedMission(int cityId, int countryId, int themeId)
        {
            var cardDisplay = new List<Card>();
            var missionList = new List<Mission>();
            var mission1 = _db.Missions.Where(x => x.DeletedAt == null && x.Status == 1 && x.CityId == cityId).AsEnumerable().ToList();

            foreach (var item in mission1)
            {
                if (!cardDisplay.Any(x => x.mission.MissionId == item.MissionId))
                {
                    for(var i = 0 ; i < (cardDisplay.Count > 3 ? 3 : cardDisplay.Count); i++)
                    {
                        cardDisplay.Add(MissionCard(item));
                    }
                }
            }
    
            homePage.card = cardDisplay;


            if (cardDisplay.Count < 3)
            {

                mission1 = _db.Missions.Where(x => x.DeletedAt == null && x.Status == 1 && x.CountryId == countryId).AsEnumerable().ToList();

                foreach (var item in mission1)
                {
                    if (!cardDisplay.Any(x => x.mission.MissionId == item.MissionId))
                    {
                        for (var i = 0; i < (cardDisplay.Count > 3 ? 3 : cardDisplay.Count); i++)
                        {
                            cardDisplay.Add(MissionCard(item));
                        }
                    }
                }

                homePage.card = cardDisplay;
            }

            if (cardDisplay.Count < 3)
            {

                mission1 = _db.Missions.Where(x => x.DeletedAt == null && x.Status == 1 && x.ThemeId == themeId).AsEnumerable().ToList();

                foreach (var item in mission1)
                {
                    if (!cardDisplay.Any(x => x.mission.MissionId == item.MissionId))
                    {
                        for (var i = 0; i < (cardDisplay.Count > 3 ? 3 : cardDisplay.Count); i++)
                        {
                            cardDisplay.Add(MissionCard(item));
                        }
                    }

                }

                homePage.card = cardDisplay;
            }

            return cardDisplay;
        }

        [HttpGet]
        public IActionResult CoWorkerStory(int id)
        {
            var story = new Story();
            story.StoryId = id;
            return PartialView("_StoryCo-WorkerPartial", story);
        }

        [HttpPost]
        public JsonResult CoWorkerStory(string InviteEmail, Story obj)
        {
            var story = _db.Stories.FirstOrDefault(x => x.StoryId == obj.StoryId);
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
            body = "<h1>" + "Shreya" + " Suggest Mission : " + story.Title + " to You</h1><br><h2><a href='https://localhost:7050'>Go Website</a></h2>";
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


    }
}
