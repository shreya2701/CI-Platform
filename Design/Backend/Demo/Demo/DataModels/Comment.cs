using System;
using System.Collections.Generic;

namespace Demo.DataModels;

public partial class Comment
{
    public long CommentId { get; set; }

    public long MissionId { get; set; }

    public long UserId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Mission Mission { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
