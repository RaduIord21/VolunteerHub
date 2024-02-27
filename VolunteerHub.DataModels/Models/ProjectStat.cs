using System;
using System.Collections.Generic;

namespace VolunteerHub.DataModels.Models;

public partial class ProjectStat
{
    public long Id { get; set; }

    public long ProjectId { get; set; }

    public long TotalTasksAsigned { get; set; }

    public long TotalTasksCompleted { get; set; }

    public long TotalTasksUncompleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Project Project { get; set; } = null!;
}
