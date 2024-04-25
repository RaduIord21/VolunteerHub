using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolunteerHub.DataModels.Models;

namespace VolunteerHub.DataAccessLayer.Interfaces
{
    public interface IProjectStatsRepository : IGenericRepository<ProjectStat>
    {
        ProjectStat? GetByProjectId(long? id);
    }
}
