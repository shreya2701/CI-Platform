using CI_Platform_Project.DataModels;

namespace CI_Platform_Project.Models
{
    public class StoryDetail
    {
        public StoryCard StoryCard { get; set; }
        public List<StoryMedium> storyMedium { get; set; }
        public Story Story { get; set; }

        public long storyId { get; set; }

        public long missionId { get; set;}
        public string storyTitle  { get; set; }
        public string storyDescription { get; set; }
        public List<string> storyImg { get; set; }

        public string userName { get; set; }
        public string userImg { get; set; }
        public int viewCount { get; set; }  

    }
}
