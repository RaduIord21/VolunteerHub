namespace VolunteerHub.Backend.Models
{
    public class ProjectTasksDto
    {
        
        public long? AssigneeId { get; set; }
        public string Name { get; set; } = null!;

        public long Description { get; set; }

        public string Action { get; set; } = null!;

        public long? SuccessTreshold { get; set; }

        public string MeasureUnit { get; set; } = null!;

        public bool IsTime { get; set; }

        public bool NeedsValidation { get; set; }
    }
}
