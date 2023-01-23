using CI_Platform_Project.DataModels;

namespace CI_Platform_Project.Models
{
    public class TimesheetVolunteer
    {
        public Timesheet timesheet { get; set; }    
        public Mission mission { get; set; }
        public List<Timesheet> timesheets { get; set; }
        public  List<Mission> missions { get; set; }
    }
}
