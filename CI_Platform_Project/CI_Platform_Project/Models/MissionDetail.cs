using CI_Platform_Project.DataModels;

namespace CI_Platform_Project.Models
{
    public class MissionDetail
    {
        public Card missionCard { get; set; }
        public List<MissionMedium> missionImages { get; set; }
        public string skills { get; set; }
        public List<RecentVolunteer> recentVolunteers { get; set; }
        public List<Card> relatedMission { get; set; }
        public int? Rating { get; set; } 
        public int applicationApply { get; set; }
        public MissionApplication MissionApplication { get; set; }

    }
}
