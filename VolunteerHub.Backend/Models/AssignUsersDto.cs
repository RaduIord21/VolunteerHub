using VolunteerHub.DataModels.Models;

namespace VolunteerHub.Backend.Models
{
    public class AssignUsersDto
    {
        public long TaskId { get; set; }
        public IList<User>? Users { get; set; }
    }
}
