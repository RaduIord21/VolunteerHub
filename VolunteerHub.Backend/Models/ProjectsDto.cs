using System.ComponentModel.DataAnnotations;

namespace VolunteerHub.Backend.Models
{
    public class ProjectsDto
    {
        [Required]
        public string ProjectName { get; set; }
        public string Description { get; set; } = null!;
        public DateTime? EndDate { get; set; }

        public long? OrganizationId { get; set; }
    }
}
