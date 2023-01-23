using CI_Platform_Project.DataModels;

namespace CI_Platform_Project.Models
{
    public class UserDetail
    {
        public User User { get; set; }
        public List<City> cities { get; set; }
        public List<Country> country { get; set; }
        public List<Skill> skills { get; set; }
        public string Avatar { get; set;}   
        public List<string>? UserSkill { get; set; }

        public string Password { get; set; }
        public string newPassword { get; set; }
    }
}
