using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace VolunteerHub.DataModels.Models;

public partial class Organization
{

    public string Name { get; set; } = null!;

    public string Adress { get; set; } = null!;

    public string Contact { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
    [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

    public int Id { get; set; }

}
