namespace VolunteerHub.Models
{
    public class UserViewModel
    {
        public partial class User
        {
            public long Id { get; set; }

            
            public string Name { get; set; } = null!;

            public string Prenume { get; set; } = null!;

            public string Email { get; set; } = null!;

            public bool Group { get; set; }

            public long OrganizationId { get; set; }

        }
    }
}
