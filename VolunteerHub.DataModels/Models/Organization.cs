using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VolunteerHub.DataModels.Models;

public partial class Organization
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Adress { get; set; }

    [Required]
    public string Contact { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; } = null!;

    public string Code { get; set; } = null!;   

    public string? OwnerId { get; set; }


    [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public virtual ICollection<Project>? Projects { get; set; }

    public virtual ICollection<UserOrganization> UserOrganizations { get; set; } = new List<UserOrganization>();

    public User? User { get; set; }
}
