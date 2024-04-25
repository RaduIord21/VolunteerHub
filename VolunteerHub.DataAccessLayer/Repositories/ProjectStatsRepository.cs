using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolunteerHub.DataAccessLayer.Interfaces;
using VolunteerHub.DataModels.Models;

namespace VolunteerHub.DataAccessLayer.Repositories
{
    public class ProjectStatsRepository : GenericRepository<ProjectStat>, IProjectStatsRepository
    {
        public ProjectStatsRepository(VolunteerHubContext context) : base(context)
        {
        }

        public ProjectStat? GetByProjectId(long? id)
        {
            if (id == null)
            {
                return null;
            }
            return context.ProjectStats.FirstOrDefault(ps => ps.ProjectId == id); 
        }
    }
}
