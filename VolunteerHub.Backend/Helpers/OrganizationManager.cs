using VolunteerHub.DataAccessLayer.Interfaces;
using VolunteerHub.DataAccessLayer.Repositories;
using VolunteerHub.DataModels.Models;

namespace VolunteerHub.Backend.Helpers
{
    public class OrganizationManager
    {
        private readonly IOrganizationRepository _organizationRepository;
        public OrganizationManager(
            IOrganizationRepository organizationRepository
            )
        {
            _organizationRepository = organizationRepository;
        }

        public long AddOrganization(Organization organization)
        {
            _organizationRepository.Add(organization);
            _organizationRepository.Save();
            return organization.Id;
        }
    }
}
