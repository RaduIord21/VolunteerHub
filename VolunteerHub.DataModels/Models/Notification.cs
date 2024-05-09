using System;
using System.Collections.Generic;

namespace VolunteerHub.DataModels.Models;

public partial class Notification
{
    public long Id { get; set; }

    public string Email { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string Subject { get; set; } = null!;

}
