using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolunteerHub.DataModels.Models
{
    public class UserTask
    {
        public long Id { get; set; }

        public string? UserId { get; set; }
        
        public long TaskId { get; set; }

        public virtual User? User { get; set; }
        public virtual ProjectTask? Task {  get; set; }


    }
}
