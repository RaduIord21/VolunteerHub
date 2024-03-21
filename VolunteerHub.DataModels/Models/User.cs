﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace VolunteerHub.DataModels.Models;

public class User : IdentityUser
{
    public long? OrganizationId { get; set; }
    public Organization? Organization { get; set; } = null!;

    public virtual ICollection<ProjectTask> ProjectTasks { get; set; } = new List<ProjectTask>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<UserStat> UserStats { get; set; } = new List<UserStat>();
}
