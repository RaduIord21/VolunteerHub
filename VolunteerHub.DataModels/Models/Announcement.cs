using System;
using System.Collections.Generic;

namespace VolunteerHub.DataModels.Models;

public partial class Announcement
{
    public long Id { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public long CreatedAt { get; set; }

    public long UpdatedAt { get; set; }

    public long ProjectId { get; set; }

    public virtual Project Project { get; set; } = null!;
}
