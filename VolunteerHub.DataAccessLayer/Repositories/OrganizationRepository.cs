using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolunteerHub.DataAccessLayer.Interfaces;
using VolunteerHub.DataModels.Models;

namespace VolunteerHub.DataAccessLayer.Repositories
{
    public class OrganizationRepository : GenericRepository<Organization>, IOrganizationRepository
    {
        public OrganizationRepository(VolunteerHubContext context) : base(context)
        {
        }

        public Organization? GetByCode(string? code)
        {
            if (code == null)
            {
                return null;
            }
            return context.Organizations.FirstOrDefault(x => x.Code.Equals(code));
        }
    }
}
