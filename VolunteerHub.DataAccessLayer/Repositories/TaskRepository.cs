using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolunteerHub.DataAccessLayer.Interfaces;
using VolunteerHub.DataModels.Models;

namespace VolunteerHub.DataAccessLayer.Repositories
{
    public class TaskRepository : GenericRepository<ProjectTask>, ITaskRepository
    {
        public TaskRepository(VolunteerHubContext context) : base(context)
        {
        }
    }
}
