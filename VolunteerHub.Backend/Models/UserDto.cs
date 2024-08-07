﻿namespace VolunteerHub.Backend.Models
{
    public class UserDto
    {
        public string Name { get; set; } = null!;

        public string Adress { get; set; } = null!;

        public string Contact { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? Code { get; set; }    
    }
}
