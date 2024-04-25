namespace VolunteerHub.Backend.Models
{
    public class AnnouncementDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public long ProjectId { get; set; }
    }
}
