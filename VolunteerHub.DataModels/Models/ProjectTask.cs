using System;
using System.Collections.Generic;

namespace VolunteerHub.DataModels.Models;

public partial class ProjectTask
{
    public long Id { get; set; }

    public long ProjectId { get; set; }

    public string? AssigneeId { get; set; }

    public string Status { get; set; } = null!;

    public long Progress { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string Description { get; set; }

    public string Action { get; set; } = null!;

    public long? SuccessTreshold { get; set; }

    public string MeasureUnit { get; set; } = null!;

    public bool IsTime { get; set; }

    public bool NeedsValidation { get; set; }

    public string Name { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;
}
