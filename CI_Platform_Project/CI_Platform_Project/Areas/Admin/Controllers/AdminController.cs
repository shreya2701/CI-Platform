using CI_Platform_Project.DataModels;
using CI_Platform_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CI_Platform_Project.Models;

namespace CI_Platform_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminController : Controller
    {


        private readonly IWebHostEnvironment _hostEnvironment;

        public AdminController(IWebHostEnvironment hostingEnvironment)
        {

            _hostEnvironment = hostingEnvironment;
        }
        public CiPlatformContext _db = new CiPlatformContext();
        public IActionResult Index()
        {
            IEnumerable<User> objUserList = _db.Users.Where(x => x.DeletedAt == null);
            return View(objUserList);
        }

        public IActionResult AddUser()
        {
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
            ViewBag.CityList = CityList;
            ViewBag.CountryList = CountryList;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddUser(User obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (obj.Password != obj.ConfirmPassword)
                {
                    TempData["errorMessage"] = " Confirm Password Not Match with Password";
                    return View();
                }
                else
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    if (file != null)
                    {
                        string fileName = Guid.NewGuid().ToString();
                        var Uploads = Path.Combine(wwwRootPath, @"images\Banners");
                        var extension = Path.GetExtension(file.FileName);

                        if (obj.Avatar != null)
                        {
                            var oldImagePath = Path.Combine(wwwRootPath, obj.Avatar.TrimStart('\\'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        using (var fileSrteam = new FileStream(Path.Combine(Uploads, fileName + extension), FileMode.Create))
                        {
                            file.CopyTo(fileSrteam);
                        }
                        obj.Avatar = @"\images\Banners\" + fileName + extension;
                    }
                    _db.Users.Add(obj);
                    _db.SaveChanges();
                    return RedirectToAction("Index", "Admin", new { Areas = "Admin" });
                }

            }
            return View(obj);
        }

        public IActionResult EditUser(int? UserId)
        {
            if (UserId == null || UserId == 0)
            {
                return NotFound();
            }
            var UserFromDb = _db.Users.FirstOrDefault(x => x.UserId == UserId);
            if (UserFromDb == null)
            {
                return NotFound();
            }
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
            ViewBag.CityList = CityList;
            ViewBag.CountryList = CountryList;
            return View(UserFromDb);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(User obj, string UserFromDb, IFormFile? file)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var Uploads = Path.Combine(wwwRootPath, @"images\Banners");
                var extension = Path.GetExtension(file.FileName);

                if (obj.Avatar != null)
                {
                    var oldImagePath = Path.Combine(wwwRootPath, obj.Avatar.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileSrteam = new FileStream(Path.Combine(Uploads, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileSrteam);
                }
                obj.Avatar = @"\images\Banners\" + fileName + extension;
            }
            _db.Users.Attach(obj);
            var entry = _db.Entry(obj);
            var data = _db.Users.Find(obj.UserId);
            data.UpdatedAt = DateTime.Now;
            _db.Users.Update(obj);
            entry.Property(e => e.CreatedAt).IsModified = false;
            _db.SaveChanges();
            return RedirectToAction("Index", "Admin", new { Areas = "Admin" });
        }

        
        
        public IActionResult OnPostDelete(int id)
        {
            var user = _db.Users.FirstOrDefault(x => x.UserId == id);
            if (user == null)
            {
                return NotFound();

            }
            user.DeletedAt = DateTime.Now;
            _db.Users.Update(user);
            _db.SaveChanges();
            return RedirectToAction("Index", "Admin", new { Areas = "Admin" });
        }



        public IActionResult Banner()
        {
            IEnumerable<Banner> ObjBannerList = _db.Banners.Where(x => x.DeletedAt == null);
            return View(ObjBannerList);
        }

        public IActionResult AddBanner()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AddBanner(int id)
        {
            var Banner = _db.Banners.FirstOrDefault(x => x.BannerId == id);
            return View(Banner);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddBanner(Banner obj, IFormFile? file)
        {

            string wwwRootPath = _hostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var Uploads = Path.Combine(wwwRootPath, @"images\Banners");
                var extension = Path.GetExtension(file.FileName);

                if (obj.Image != null)
                {
                    var oldImagePath = Path.Combine(wwwRootPath, obj.Image.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileSrteam = new FileStream(Path.Combine(Uploads, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileSrteam);
                }
                obj.Image = @"\images\Banners\" + fileName + extension;
            }
            //Insert Record
            if (obj.BannerId == 0)
            {
                _db.Banners.Add(obj);
            }
            else
            {

                obj.UpdatedAt = DateTime.Now;
                _db.Banners.Update(obj);

            }
            _db.SaveChanges();
            return RedirectToAction("Banner", "Admin", new { Areas = "Admin" });
        }

        [HttpDelete]
        [HttpPost]
        public IActionResult DeleteBanner(int? id)
        {
            var banner = _db.Banners.FirstOrDefault(x => x.BannerId == id);
            if (banner == null)
            {
                return NotFound();

            }
            banner.DeletedAt = DateTime.Now;
            _db.Banners.Update(banner);
            _db.SaveChanges();
            return RedirectToAction("Banner", "Admin", new { Areas = "Admin" });
        }

        public IActionResult CMS()
        {
            IEnumerable<CmsPage> objCMSList = _db.CmsPages.Where(x => x.DeletedAt == null);
            return View(objCMSList);
        }

        public IActionResult UpsertCMS()
        {
            return View();
        }
        [HttpGet]
        public IActionResult UpsertCMS(int id)
        {
            var CMS = _db.CmsPages.FirstOrDefault(x => x.CmsPageId == id);
            return View(CMS);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpsertCMS(CmsPage obj)
        {
            //Insert Record
            if (obj.CmsPageId == 0)
            {
                _db.CmsPages.Add(obj);
            }
            else
            {

                obj.UpdatedAt = DateTime.Now;
                _db.CmsPages.Update(obj);

            }
            _db.SaveChanges();
            return RedirectToAction("CMS", "Admin", new { Areas = "Admin" });
        }

        [HttpDelete]
        [HttpPost]
        public IActionResult DeleteCMS(int? id)
        {
            var CMS = _db.CmsPages.FirstOrDefault(x => x.CmsPageId == id);
            if (CMS == null)
            {
                return NotFound();

            }
            CMS.DeletedAt = DateTime.Now;
            _db.CmsPages.Update(CMS);
            _db.SaveChanges();
            return RedirectToAction("CMS", "Admin", new { Areas = "Admin" });
        }


        public IActionResult MissionTheme()
        {
            IEnumerable<MissionTheme> objThemeList = _db.MissionThemes.Where(x => x.DeletedAt == null);
            return View(objThemeList);
        }

        public IActionResult UpsertTheme()
        {
            return View();
        }
        [HttpGet]
        public IActionResult UpsertTheme(int id)
        {
            var Theme = _db.MissionThemes.FirstOrDefault(x => x.MissionThemeId == id);
            return View(Theme);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpsertTheme(MissionTheme obj)
        {
            //Insert Record
            if (obj.MissionThemeId == 0)
            {
                _db.MissionThemes.Add(obj);
            }
            else
            {

                obj.UpdatedAt = DateTime.Now;
                _db.MissionThemes.Update(obj);

            }
            _db.SaveChanges();
            return RedirectToAction("MissionTheme", "Admin", new { Areas = "Admin" });
        }

        [HttpDelete]
        [HttpPost]
        public IActionResult DeleteTheme(int? id)
        {
            var Theme = _db.MissionThemes.FirstOrDefault(x => x.MissionThemeId == id);
            if (Theme == null)
            {
                return NotFound();

            }
            Theme.DeletedAt = DateTime.Now;
            if (Theme.Status == 1)
            {
                Theme.Status = (Theme.Status = 0);
            }
            var x = _db.Missions.FirstOrDefault(x => x.ThemeId == id);
            x.DeletedAt = DateTime.Now;
            _db.Missions.Update(x);
            _db.MissionThemes.Update(Theme);
            _db.SaveChanges();
            return RedirectToAction("MissionTheme", "Admin", new { Areas = "Admin" });
        }



        public IActionResult MissionSkill()
        {
            IEnumerable<Skill> objSkillList = _db.Skills.Where(x => x.DeletedAt == null);
            return View(objSkillList);
        }

        public IActionResult UpsertSkill()
        {
            return View();
        }
        [HttpGet]
        public IActionResult UpsertSkill(int id)
        {
            var Skill = _db.Skills.FirstOrDefault(x => x.SkillId == id);
            return View(Skill);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpsertSkill(Skill obj)
        {
            //Insert Record
            if (obj.SkillId == 0)
            {
                _db.Skills.Add(obj);
            }
            else
            {

                obj.UpdatedAt = DateTime.Now;
                _db.Skills.Update(obj);

            }
            _db.SaveChanges();
            return RedirectToAction("MissionSkill", "Admin", new { Areas = "Admin" });
        }

        [HttpDelete]
        [HttpPost]
        public IActionResult DeleteSkill(int? id)
        {
            var Skill = _db.Skills.FirstOrDefault(x => x.SkillId == id);
            if (Skill == null)
            {
                return NotFound();

            }
            Skill.DeletedAt = DateTime.Now;
            _db.Skills.Update(Skill);
            _db.SaveChanges();
            return RedirectToAction("MissionSkill", "Admin", new { Areas = "Admin" });
        }


        public IActionResult Mission()
        {
            IEnumerable<Mission> objMissionList = _db.Missions.Where(x => x.DeletedAt == null);
            return View(objMissionList);
        }

        public IActionResult UpsertMission()
        {
            return View();
        }


        [HttpGet]
        public IActionResult UpsertMission(int id)
        {
            MissionZm missionZm = new();
            Mission mission = new();

            IEnumerable<SelectListItem> ThemeList = _db.MissionThemes.Where(x => x.DeletedAt == null && x.Status == 1).Select(
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
            if (id == null || id == 0)
            {

                ViewBag.ThemeList = ThemeList;
                ViewBag.skilllist = skilllist;
                //ViewBag.CityList = CityList;
                ViewBag.CountryList = CountryList;
                return View();
            }
            else
            {
                MissionZm x = new MissionZm();
                x.mission = _db.Missions.FirstOrDefault(x => x.MissionId == id);
                x.missionMedium = _db.MissionMedia.FirstOrDefault(x => x.MissionId == id);
                x.missionDocument = _db.MissionDocuments.FirstOrDefault(x => x.MissionId == id);
                x.goal = _db.GoalMissions.FirstOrDefault(x => x.MissionId == x.Mission.MissionId);

                x.addSkill = _db.MissionSkills.Where(s => s.MissionId == x.mission.MissionId).Select(x => x.SkillId.ToString()).ToList();
                ViewBag.ThemeList = ThemeList;
                ViewBag.CityList = CityList;
                ViewBag.skilllist = skilllist;
                ViewBag.CountryList = CountryList;
                return View(x);
            }
        }
        public JsonResult City_Bind(int CityId)
        {
            IEnumerable<SelectListItem> CityList = _db.Cities.Where(x => x.DeletedAt == null && x.CountryId == CityId).Select(
                u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.CityId.ToString()
                }
            );
            CityList = CityList.ToList();
            return (Json(CityList));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpsertMission(List<IFormFile> file, List<IFormFile> document, MissionZm model)
        {
            //Insert Record


            //Insert Record
            if (model.mission.MissionId == 0)
            {
                _db.Missions.Add(model.mission);
                _db.SaveChanges();

                MissionMedium missionMedium = new MissionMedium();
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if (file != null)
                {
                    foreach (var x in file)
                    {
                        var missionMedia = new MissionMedium();
                        missionMedia.MissionId = model.mission.MissionId;
                        missionMedia.MediaType = Path.GetExtension(x.FileName); ;
                        missionMedia.MediaName = x.FileName;
                        missionMedia.MediaPath = img(x);
                        _db.MissionMedia.Add(missionMedia);
                        _db.SaveChanges();
                    }
                }

                if (document != null)
                {
                    foreach (var x in document)
                    {
                        var missionDocument = new MissionDocument();
                        missionDocument.MissionId = model.mission.MissionId;
                        missionDocument.DocumentType = Path.GetExtension(x.FileName); ;
                        missionDocument.DocumentName = x.FileName;
                        missionDocument.DocumentPath = img(x);
                        _db.MissionDocuments.Add(missionDocument);
                        _db.SaveChanges();
                    }
                }

                if (model.mission.MissionType == 2)
                {
                    var goal = new GoalMission();
                    goal.MissionId = model.mission.MissionId;
                    goal.GoalValue = model.goal.GoalValue;
                    goal.GoalObjectiveText = model.goal.GoalObjectiveText;

                    _db.GoalMissions.Add(goal);
                    _db.SaveChanges();
                }


                if (model.addSkill != null)
                {
                    //var x = model.addSkill[0].Split(',').Select(Int32.Parse).ToList();
                    foreach (var n in model.addSkill)
                    {
                        var missionSkill = new MissionSkill();
                        missionSkill.MissionId = model.mission.MissionId;
                        missionSkill.SkillId = int.Parse(n);

                        _db.MissionSkills.Add(missionSkill);
                        _db.SaveChanges();
                    }
                }

            }
            else
            {

                model.mission.UpdatedAt = DateTime.Now;
                _db.Missions.Update(model.mission);
                _db.SaveChanges();


                MissionMedium missionMedium = new MissionMedium();
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if (file != null)
                {
                    foreach (var x in file)
                    {
                        var missionMedia = new MissionMedium();
                        missionMedia.MissionId = model.mission.MissionId;
                        missionMedia.MediaType = Path.GetExtension(x.FileName); ;
                        missionMedia.MediaName = x.FileName;
                        missionMedia.MediaPath = img(x);
                        missionMedia.UpdatedAt = DateTime.Now;
                        _db.MissionMedia.Add(missionMedia);
                        _db.SaveChanges();
                    }
                }

                if (document != null)
                {
                    foreach (var x in document)
                    {
                        var missionDocument = new MissionDocument();
                        missionDocument.MissionId = model.mission.MissionId;
                        missionDocument.DocumentType = Path.GetExtension(x.FileName); ;
                        missionDocument.DocumentName = x.FileName;
                        missionDocument.DocumentPath = img(x);
                        missionDocument.UpdatedAt = DateTime.Now;
                        _db.MissionDocuments.Add(missionDocument);
                        _db.SaveChanges();
                    }
                }

                if (model.mission.MissionType == 2)
                {
                    var goal = new GoalMission();
                    goal.MissionId = model.mission.MissionId;
                    goal.GoalValue = model.goal.GoalValue;
                    goal.GoalObjectiveText = model.goal.GoalObjectiveText;
                    goal.UpdatedAt = model.goal.UpdatedAt;

                    _db.GoalMissions.Update(goal);
                    _db.SaveChanges();
                }




                if (model.addSkill != null)
                {
                    _db.MissionSkills.RemoveRange(_db.MissionSkills.Where(x => x.MissionId == model.mission.MissionId));
                    _db.SaveChanges();
                    //var x = model.addSkill[0].Split(',').Select(Int32.Parse).ToList();
                    foreach (var n in model.addSkill)
                    {
                        var missionSkill = new MissionSkill();
                        missionSkill.MissionId = model.mission.MissionId;
                        missionSkill.SkillId = int.Parse(n);
                        missionSkill.UpdatedAt = DateTime.Now;

                        _db.MissionSkills.Update(missionSkill);
                        _db.SaveChanges();
                    }
                }

            }

            return RedirectToAction("Mission", "Admin", new { Areas = "Admin" });
        }



        public IActionResult DeleteMission(int? id)
        {
            //var mission = _db.Missions.FirstOrDefault(x => x.MissionId == id);
            //mission.DeletedAt = DateTime.Now;
            //_db.Missions.Update(mission);
            //_db.SaveChanges();
            //return RedirectToAction("Mission", "Admin", new { Areas = "Admin" });


            _db.MissionDocuments.RemoveRange(_db.MissionDocuments.Where(x => x.MissionId == id));
            _db.MissionMedia.RemoveRange(_db.MissionMedia.Where(x => x.MissionId == id));
            _db.MissionSkills.RemoveRange(_db.MissionSkills.Where(x => x.MissionId == id));

            var mission = _db.Missions.FirstOrDefault(x => x.MissionId == id);
            mission.DeletedAt = DateTime.Now;
            _db.Missions.Update(mission);

            if (mission.MissionType == 2)
            {
                var goalMission = _db.GoalMissions.FirstOrDefault(x => x.MissionId == id);
                goalMission.DeletedAt = DateTime.Now;
                _db.GoalMissions.Update(goalMission);
            }
            _db.SaveChanges();


            return RedirectToAction("Mission", "Admin", new { Areas = "Admin" });
        }

        public string img(IFormFile file)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;

            string fileName = Guid.NewGuid().ToString();
            var Uploads = Path.Combine(wwwRootPath, @"images\MissionImage");
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
            return @"\images\MissionImage\" + fileName + extension;
        }


        public IActionResult MissionApplication()
        {
            var x = new AdminZm();
            x.missionApplication = _db.MissionApplications.Where(x => x.DeletedAt == null && x.ApprovalStatus == 0).AsEnumerable().ToList();
            x.User = _db.Users.AsEnumerable().ToList();
            x.mission = _db.Missions.Where(w => w.DeletedAt == null).AsEnumerable().ToList();

            return View(x);
        }

        public IActionResult MissionAppAccept(int id)
        {
            var missionApp = _db.MissionApplications.FirstOrDefault(v => v.MissionApplicationId == id);
            missionApp.ApprovalStatus = 1;
            _db.MissionApplications.Update(missionApp);
            _db.SaveChanges();

            return RedirectToAction("MissionApplication", "Admin", new { Areas = "Admin" });
        }

        public IActionResult MissionAppDecline(int id)
        {
            var missionApp = _db.MissionApplications.FirstOrDefault(v => v.MissionApplicationId == id);
            missionApp.ApprovalStatus = 0;
            if (missionApp == null)
            {
                return NotFound();

            }
            _db.MissionApplications.Update(missionApp);
            _db.SaveChanges();
            return RedirectToAction("MissionApplication", "Admin", new { Areas = "Admin" });
        }

        public IActionResult Story()
        {
            var x = new AdminZm();
            x.stories = _db.Stories.Where(x => x.DeletedAt == null).AsEnumerable().ToList();
            x.User = _db.Users.AsEnumerable().ToList();
            x.mission = _db.Missions.AsEnumerable().ToList();

            return View(x);
        }


        public IActionResult StoryPublish(int id)
        {
            var story = _db.Stories.FirstOrDefault(v => v.StoryId == id);
            story.Status = 1;
            _db.Stories.Update(story);
            _db.SaveChanges();

            return RedirectToAction("Story", "Admin", new { Areas = "Admin" });
        }

        public IActionResult StoryDecline(int id)
        {
            var story = _db.Stories.FirstOrDefault(v => v.StoryId == id);
            story.Status = 0;
            if (story == null)
            {
                return NotFound();

            }
            _db.Stories.Update(story);
            _db.SaveChanges();
            return RedirectToAction("Story", "Admin", new { Areas = "Admin" });
        }


        [HttpDelete]
        [HttpPost]
        public IActionResult DeleteStory(int? id)
        {
            var story = _db.Stories.FirstOrDefault(x => x.StoryId == id);
            if (story == null)
            {
                return NotFound();

            }
            story.DeletedAt = DateTime.Now;
            story.Status = 0;
            _db.Stories.Update(story);
            _db.SaveChanges();
            return RedirectToAction("Story", "Admin", new { Areas = "Admin" });
        }

        public IActionResult logOut()
        {
            return RedirectToAction("Login", "User", new { areas = "Customer" });
        }


        //public IActionResult StoryDetail(int? id)
        //{
        //    var x = new AdminZm();
        //    x.stories = _db.Stories.FirstOrDefault(x => x.DeletedAt == null && x.StoryId == id);
        //    x.User = _db.Users.Where(u => u.DeletedAt == null).AsEnumerable().ToList();
        //    x.mission = _db.Missions.Where(w => w.DeletedAt == null).AsEnumerable().ToList();
        //    x.storyMedia = _db.StoryMedia.Where(w => w.DeletedAt == null).AsEnumerable().ToList();

        //    return View(x);
        //}
    }
}
