using CI_Platform_Project.DataModels;

namespace CI_Platform_Project.Models
{
    public class AdminZm
    {
        public List<User>? User { get; set; }

        public List<Mission>? mission { get; set; }
         
        public List<MissionApplication>? missionApplication { get; set; }

        public List<Story>? stories { get; set; }

        public List<StoryMedium>? storyMedia { get; set; }

        public String? StoryMedium { get; set; }


    }
}
