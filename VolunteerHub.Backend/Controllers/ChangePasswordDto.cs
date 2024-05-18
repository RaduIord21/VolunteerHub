namespace VolunteerHub.Backend.Controllers
{
    public class ChangePasswordDto
    {
        public string? newPassword {  get; set; }

        public string? oldPassword { get; set; }

        public string? confirmPassword { get; set; }
    }
}