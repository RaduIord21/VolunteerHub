namespace VolunteerHub.Backend.Models
{
    public class ProjectUserDto
    {
        public long? ProjectId {  get; set; }
        public List<string>? UserIds { get; set; } 
       
    }
}
