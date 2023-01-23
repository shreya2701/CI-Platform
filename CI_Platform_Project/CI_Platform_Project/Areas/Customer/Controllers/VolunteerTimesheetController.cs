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
        [HttpPost]
        public IActionResult AddTimeTimesheet( TimesheetVolunteer obj)
        {
             
                var timesheet = new Timesheet();
                timesheet = obj.timesheet;
                timesheet.UserId = int.Parse(HttpContext.Session.GetString("UserId"));
                if (obj.timesheet.TimesheetId == 0)
                {
                    timesheet.Status = 1;
                    _db.Timesheets.Add(timesheet);
                }
                _db.SaveChanges();
            
            return RedirectToAction("VolunteerTimesheet", "VolunteerTimesheet", new { Areas = "Customer" });
        }
    }
}   

