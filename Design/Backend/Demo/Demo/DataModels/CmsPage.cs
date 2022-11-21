using System;
using System.Collections.Generic;

namespace Demo.DataModels;

public partial class CmsPage
{
    public long CmsPageId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string Slug { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}
