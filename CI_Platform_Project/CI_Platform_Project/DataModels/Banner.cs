using System;
using System.Collections.Generic;

namespace CI_Platform_Project.DataModels;

public partial class Banner
{
    public long BannerId { get; set; }

    public string Image { get; set; } = null!;

    public string? Text { get; set; }

    public string? Title { get; set; }

    public int Status { get; set; }

    public int? SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}
