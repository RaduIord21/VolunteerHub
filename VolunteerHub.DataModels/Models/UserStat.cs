using System;
using System.Collections.Generic;

namespace VolunteerHub.DataModels.Models;

public partial class UserStat
{
    public long Id { get; set; }

    public string UserId { get; set; } = null!;

    public long TasksCompleted { get; set; }

    public long TasksAsigned { get; set; }


}
