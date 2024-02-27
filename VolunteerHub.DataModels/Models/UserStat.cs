using System;
using System.Collections.Generic;

namespace VolunteerHub.DataModels.Models;

public partial class UserStat
{
    public long Id { get; set; }

    public string UserId { get; set; }

    public long TasksCompleted { get; set; }

    public long TasksAsigned { get; set; }

    public long TasksUncompleted { get; set; }

    public virtual User User { get; set; } = null!;
}
