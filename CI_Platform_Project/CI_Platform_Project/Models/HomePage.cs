using CI_Platform_Project.DataModels;

namespace CI_Platform_Project.Models
{
    public class HomePage
    {
        public Mission mission { get; set; }
        public List<Card> card { get; set; }     
        public User user { get; set; }


    }
}
