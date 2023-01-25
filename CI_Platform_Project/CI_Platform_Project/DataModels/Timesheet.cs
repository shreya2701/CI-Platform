using System;
using System.Collections.Generic;

namespace CI_Platform_Project.DataModels;

public partial class Timesheet
{
    public long TimesheetId { get; set; }

    public long? UserId { get; set; }

    public long MissionId { get; set; }

    public DateTime DateVolunteered { get; set; }

    public string? Notes { get; set; }

    public int Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? Hour { get; set; }

    public int? Minute { get; set; }

    public int? Action { get; set; }

    public virtual Mission Mission { get; set; } = null!;

    public virtual User? User { get; set; }
}
