using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace VolunteerHub.DataModels.Models;

public class User : IdentityUser
{
    public long ? ProjectId { get; set; }
    public virtual Project? Project { get; set; }   
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    public virtual ICollection<UserStat> UserStats { get; set; } = new List<UserStat>();
    public virtual ICollection<UserTask> UserTasks { get; set; } = new List<UserTask>();
    public ICollection<Organization>? Organizations { get; set; }
    public virtual ICollection<UserOrganization> UserOrganizations { get; set; } = new List<UserOrganization>();


}
