using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace VolunteerHub.DataModels.Models;

public class User : IdentityUser
{
    public virtual Organization? Organization { get; set; }

    public virtual ICollection<ProjectTask> ProjectTasks { get; set; } = new List<ProjectTask>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<UserStat> UserStats { get; set; } = new List<UserStat>();
}
