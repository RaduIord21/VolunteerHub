using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace VolunteerHub.DataModels.Models;

public partial class ProjectTask
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public string Status { get; set; } = null!;
    public decimal Progress { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public string Description { get; set; }

    public string Action { get; set; } = null!;

    public long? SuccessTreshold { get; set; }

    public string MeasureUnit { get; set; } = null!;

    public bool IsTime { get; set; }

    public string Name { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;
    public virtual ICollection<UserTask> UserTasks { get; set; } = new List<UserTask>();

}
