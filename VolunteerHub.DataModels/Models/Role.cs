using System;
using System.Collections.Generic;

namespace VolunteerHub.DataModels.Models;

public partial class Role
{
    public long Id { get; set; }

    public string Role1 { get; set; } = null!;

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
