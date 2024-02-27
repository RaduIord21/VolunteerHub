using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VolunteerHub.DataModels.Models;

namespace VolunteerHub.DataAccessLayer.Interfaces
{
    public interface ITaskRepository : IGenericRepository<ProjectTask>
    {
    }
}
