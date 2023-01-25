using CI_Platform_Project.DataModels;

namespace CI_Platform_Project.Models
{
    public class Card
    {
        public Mission? mission { get; set; } 
        public String theme { get; set; }
        public String img { get; set; }
        public int seatleft { get; set; }
        public String city { get; set; }
        public GoalMission GoalMission { get; set; }

        public int? favorite { get; set; }

        public User user { get; set; }
        public MissionApplication missionApplication { get; set; }
       
    }
}
