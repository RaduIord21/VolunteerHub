namespace VolunteerHub.Backend.Models
{
    public class ProjectTasksDto
    {
        public string Name { get; set; } = null!;
        public long? ProjectId { get; set; }
        public string Description { get; set; }
        public string Action { get; set; } = null!;
        public DateTime? EndDate {  get; set; } 
        public long? SuccessTreshold { get; set; }

        public string MeasureUnit { get; set; } = null!;

        public bool IsTime { get; set; }

    }
}
