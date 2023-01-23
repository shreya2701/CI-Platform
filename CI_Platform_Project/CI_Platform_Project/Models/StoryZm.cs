using CI_Platform_Project.DataModels;

namespace CI_Platform_Project.Models
{
    public class StoryZm
    {
        public Story story { get; set; }
        public StoryMedium storyMedium { get; set; }
        public StoryInvite storyInvite { get; set; }

        public List<StoryCard> storyCard { get; set; }
    }
}
