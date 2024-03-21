﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolunteerHub.DataModels.Models;

namespace VolunteerHub.DataAccessLayer.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        public User? GetByEmail(string email);
        public Task<User> GetByEmaiAsync(string email);


    }
}
