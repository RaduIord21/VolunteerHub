using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolunteerHub.DataAccessLayer.Interfaces;
using VolunteerHub.DataModels.Models;

namespace VolunteerHub.DataAccessLayer.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly VolunteerHubContext _context;
        public UserRepository(VolunteerHubContext context) : base(context)
        {
            _context = context;
        }

        public Task<User> GetByEmaiAsync(string email)
        {
            throw new NotImplementedException();
        }

        public User? GetByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }
       
    }
}
