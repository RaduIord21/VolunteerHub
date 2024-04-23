namespace VolunteerHub.Backend.Models
{
    public class UpdateTaskDto
    {
        public long Id { get; set; }
        public decimal Progress { get; set; }
        public DateTime SubmissionDate{ get; set; }
    }
}
