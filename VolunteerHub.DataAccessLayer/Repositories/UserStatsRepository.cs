using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolunteerHub.DataAccessLayer.Interfaces;
using VolunteerHub.DataModels.Models;

namespace VolunteerHub.DataAccessLayer.Repositories
{
    public class UserStatsRepository : GenericRepository<UserStat>, IUserStatsRepository
    {
        public UserStatsRepository(VolunteerHubContext context) : base(context)
        {
        }

        public UserStat? GetByUserId(string? id)
        {
            if (id == null)
            {
                return null;
            }
            return context.UserStats.FirstOrDefault(us => us.UserId.Equals(id));
        }   
    }
}
