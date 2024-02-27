using System;
using System.Collections.Generic;

namespace VolunteerHub.DataModels.Models;

public partial class Project
{
    public long Id { get; set; }

    public long OrganizationId { get; set; }

    public long UserId { get; set; }

    public long GoalName { get; set; }

    public string GoalCode { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime? EndDate { get; set; }

    public string OwnerId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();

    public virtual User Owner { get; set; } = null!;

    public virtual ICollection<ProjectStat> ProjectStats { get; set; } = new List<ProjectStat>();

    public virtual ICollection<ProjectTask> ProjectTasks { get; set; } = new List<ProjectTask>();
}
