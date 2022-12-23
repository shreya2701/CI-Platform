using CI_Platform_Project.DataModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CI_Platform_Project.Models
{
    public class MissionZm
    {
        public Mission? mission { get; set; }
        public User? user { get; set; }
         
        public MissionMedium? missionMedium { get; set; }

        public MissionDocument? missionDocument { get; set; }
        public GoalMission? goal { get; set; }

        public List<SelectListItem>? CityId { get; set; }

        public List<SelectListItem>? CountryId { get; set; }

        public List<SelectListItem>? skills { get; set; }

        public MissionTheme? themes { get; set; }

        public List<Mission>? missions { get; set; }



        public List<string>? addSkill { get; set; }

        public String? MissionMediaId { get; set; }
    }
}
