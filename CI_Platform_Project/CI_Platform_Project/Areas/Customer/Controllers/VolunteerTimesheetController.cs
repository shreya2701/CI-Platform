using CI_Platform_Project.DataModels;
using CI_Platform_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CI_Platform_Project.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class VolunteerTimesheetController : Controller
    {
        public CiPlatformContext _db = new CiPlatformContext();
        public IActionResult VolunteerTimesheet()
        {
            var timesheet = new TimesheetVolunteer();
            //timesheet.timesheets = _db.Timesheets.Where(x => x.UserId == int.Parse(HttpContext.Session.GetString("UserId")) && x.DeletedAt == null).ToList();
            timesheet.missions = _db.Missions.ToList();
            IEnumerable<Timesheet> objtimesheet = _db.Timesheets.Where(x => x.UserId == int.Parse(HttpContext.Session.GetString("UserId")) && x.DeletedAt == null);
            return View(objtimesheet);
            //return View(timesheet);
        }
        [HttpGet]
        public IActionResult AddTimeTimesheet()
        {
            var timesheet = new List<Timesheet>();
            var TimesheetVolunteer = new TimesheetVolunteer();
            TimesheetVolunteer.timesheets = timesheet;
            var temp = _db.MissionApplications.Where(x => x.UserId == int.Parse(HttpContext.Session.GetString("UserId")) && x.DeletedAt == null && x.Mission.MissionType == 1).Select(x => x.MissionId).ToList();
            foreach (var item in temp)
            {
                IEnumerable<SelectListItem> mission = _db.Missions.Where(x => x.MissionId == item && x.DeletedAt == null).Select(
                    u => new SelectListItem
                    {
                        Text = u.Title,
                        Value = u.MissionId.ToString()
                    }
                );

                ViewBag.mission = mission;
            }
            return PartialView("_TimesheetPartial", TimesheetVolunteer);

        }
        public IActionResult EditTimeTimesheet(int id)
        {
            var timesheet = new Timesheet();
            var TimesheetVolunteer = new TimesheetVolunteer();
            var temp1 = _db.Timesheets.FirstOrDefault(x => x.TimesheetId == id);
            TimesheetVolunteer.timesheet = temp1;
            var temp = _db.MissionApplications.Where(x => x.UserId == int.Parse(HttpContext.Session.GetString("UserId")) && x.DeletedAt == null && x.Mission.MissionType == 1).Select(x => x.MissionId).ToList();
            foreach (var item in temp)
            {
                IEnumerable<SelectListItem> mission = _db.Missions.Where(x => x.MissionId == item && x.DeletedAt == null).Select(
                    u => new SelectListItem
                    {
                        Text = u.Title,
                        Value = u.MissionId.ToString()
                    }
                );

                ViewBag.mission = mission;
            }
            return PartialView("_TimesheetPartial", TimesheetVolunteer);
        }
        [HttpPost]
        public IActionResult AddTimeTimesheet(TimesheetVolunteer obj)
        {
            if (obj.timesheet.MissionId != 0 && obj.timesheet.DateVolunteered != DateTime.MinValue && (obj.timesheet.Hour != null || obj.timesheet.Minute != null))
            {
                if (obj.timesheet.Minute != null && obj.timesheet.Minute / 60 > 0)
                {
                    obj.timesheet.Hour = obj.timesheet.Hour + (int)(obj.timesheet.Minute / 60);
                    obj.timesheet.Minute = obj.timesheet.Minute % 60;
                }

                var timesheet = new Timesheet();
                timesheet = obj.timesheet;
                timesheet.UserId = int.Parse(HttpContext.Session.GetString("UserId"));
                if (obj.timesheet.TimesheetId == 0)
                {
                    timesheet.Status = 1;
                    _db.Timesheets.Add(timesheet);
                }
                else
                {
                    timesheet.UpdatedAt = DateTime.Now;
                    _db.Timesheets.Update(timesheet);
                }
                _db.SaveChanges();
            }

            return RedirectToAction("VolunteerTimesheet", "VolunteerTimesheet", new { Areas = "Customer" });
        }


        [HttpGet]
        public IActionResult AddGoalTimesheet()
        {
            var timesheet = new List<Timesheet>();
            var TimesheetVolunteer = new TimesheetVolunteer();
            TimesheetVolunteer.timesheets = timesheet;
            var temp = _db.MissionApplications.Where(x => x.UserId == int.Parse(HttpContext.Session.GetString("UserId")) && x.DeletedAt == null && x.Mission.MissionType == 2).Select(x => x.MissionId).ToList();
            foreach (var item in temp)
            {
                IEnumerable<SelectListItem> mission = _db.Missions.Where(x => x.MissionId == item && x.DeletedAt == null).Select(
                    u => new SelectListItem
                    {
                        Text = u.Title,
                        Value = u.MissionId.ToString()
                    }
                );

                ViewBag.mission = mission;
            }
            return PartialView("_GoalTimesheetPartial", TimesheetVolunteer);

        }
        public IActionResult EditGoalTimesheet(int id)
        {
            var timesheet = new Timesheet();
            var TimesheetVolunteer = new TimesheetVolunteer();
            var temp1 = _db.Timesheets.FirstOrDefault(x => x.TimesheetId == id);
            TimesheetVolunteer.timesheet = temp1;
            var temp = _db.MissionApplications.Where(x => x.UserId == int.Parse(HttpContext.Session.GetString("UserId")) && x.DeletedAt == null && x.Mission.MissionType == 1).Select(x => x.MissionId).ToList();
            foreach (var item in temp)
            {
                IEnumerable<SelectListItem> mission = _db.Missions.Where(x => x.MissionId == item && x.DeletedAt == null).Select(
                    u => new SelectListItem
                    {
                        Text = u.Title,
                        Value = u.MissionId.ToString()
                    }
                );

                ViewBag.mission = mission;
            }
            return PartialView("_GoalTimesheetPartial", TimesheetVolunteer);
        }
        [HttpPost]
        public IActionResult AddGoalTimesheet(TimesheetVolunteer obj)
        {
            if (obj.timesheet.MissionId != 0 && obj.timesheet.DateVolunteered != DateTime.MinValue && obj.timesheet.Action != null)
            { 

                var timesheet = new Timesheet();
                timesheet = obj.timesheet;
                timesheet.UserId = int.Parse(HttpContext.Session.GetString("UserId"));
                if (obj.timesheet.TimesheetId == 0)
                {
                    timesheet.Status = 1;
                    _db.Timesheets.Add(timesheet);
                }
                else
                {
                    timesheet.UpdatedAt = DateTime.Now;
                    _db.Timesheets.Update(timesheet);
                }
                _db.SaveChanges();
            }

            return RedirectToAction("VolunteerTimesheet", "VolunteerTimesheet", new { Areas = "Customer" });
        }

        public IActionResult DeleteTimeSheet(int id)
        {
            var timesheet = _db.Timesheets.FirstOrDefault(x => x.TimesheetId == id);
            timesheet.DeletedAt = DateTime.Now;
            timesheet.Status = 0;
            _db.Timesheets.Update(timesheet);
            _db.SaveChanges();

            return RedirectToAction("VolunteerTimesheet", "VolunteerTimesheet", new { Areas = "Customer" });
        }
    }
}

