using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolunteerHub.DataModels.Models
{
    public class UserOrganization
    {
        public long Id { get; set; }

        public long? OrganizationId { get; set;}

        public string? UserId { get; set; }

        public virtual User? User { get; set; }

        public virtual Organization? Organization { get; set; }
    }
}
