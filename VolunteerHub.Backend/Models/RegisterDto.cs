namespace VolunteerHub.Backend.Models
{
    //public class UserViewModel : Controller - aici e bugul
    public class RegisterDto
    {
        public string Username { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public long? OrganizationId { get; set; }

    }
}

