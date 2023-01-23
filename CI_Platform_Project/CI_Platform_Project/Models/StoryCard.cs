using CI_Platform_Project.DataModels;

namespace CI_Platform_Project.Models
{
    public class StoryCard
    {
        public Story Stories { get; set; }

        public Mission mission { get; set; }
        public String img { get; set; }

        public String theme { get; set; }

       public string UserImg { get; set; }
        public string UserName { get; set; }
    }
}
