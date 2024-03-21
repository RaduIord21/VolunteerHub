using Microsoft.AspNetCore.Mvc;

namespace VolunteerHub.Backend.Services.Interfaces
{
    public interface IOrganizationService
    {
        public IActionResult GetUserWithOrganization();
    }
}
