using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace VolunteerHub.DataModels.Models;

public class User : IdentityUser
{
    public long? OrganizationId { get; set; }

    public long ? ProjectId { get; set; }
    public virtual Organization? Organization { get; set; } = null!;
    public virtual Project? Project { get; set; }   
    public virtual ICollection<ProjectTask> ProjectTasks { get; set; } = new List<ProjectTask>();
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    public virtual ICollection<UserStat> UserStats { get; set; } = new List<UserStat>();
}
