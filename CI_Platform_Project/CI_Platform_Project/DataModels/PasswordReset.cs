using System;
using System.Collections.Generic;

namespace CI_Platform_Project.DataModels;

public partial class PasswordReset
{
    public int PasswordResetId { get; set; }

    public string Email { get; set; } = null!;

    public string Token { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
