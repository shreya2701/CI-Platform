using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CI_Platform_Project.DataModels;

public partial class User
{
    public long UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int PhoneNumber { get; set; }

    public string? Avatar { get; set; }

    public string? WhyIVolunteer { get; set; }

    public string? EmployeeId { get; set; }

    public string? Department { get; set; }

    public long? CityId { get; set; }

    public long? CountryId { get; set; }

    public string? ProfileText { get; set; }

    public string? LinkedInUrl { get; set; }

    public string? Title { get; set; }

    public int Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [Required(ErrorMessage = "Confirm Password required")]
    [CompareAttribute("Password", ErrorMessage = "Password doesn't match.")]
    public string ConfirmPassword { get; set; } = null!;

    public virtual City? City { get; set; }

    public virtual ICollection<Comment> Comments { get; } = new List<Comment>();

    public virtual Country? Country { get; set; }

    public virtual ICollection<FavoriteMission> FavoriteMissions { get; } = new List<FavoriteMission>();

    public virtual ICollection<MissionApplication> MissionApplications { get; } = new List<MissionApplication>();

    public virtual ICollection<MissionInvite> MissionInviteFromUsers { get; } = new List<MissionInvite>();

    public virtual ICollection<MissionInvite> MissionInviteToUsers { get; } = new List<MissionInvite>();

    public virtual ICollection<MissionRating> MissionRatings { get; } = new List<MissionRating>();

    public virtual ICollection<Story> Stories { get; } = new List<Story>();

    public virtual ICollection<Timesheet> Timesheets { get; } = new List<Timesheet>();

    public virtual ICollection<UserSkill> UserSkills { get; } = new List<UserSkill>();
}
