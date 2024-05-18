namespace VolunteerHub.Backend.Models
{
    public class EditTasksDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; }
        public string Action { get; set; } = null!;
        public DateTime? EndDate {  get; set; } 
        public long? SuccessTreshold { get; set; }
        public string MeasureUnit { get; set; }
        public bool IsTime { get; set; }
    }
}
