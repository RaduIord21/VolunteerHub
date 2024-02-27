using System.Numerics;
using VolunteerHub.Backend.Models;
using VolunteerHub.DataModels.Models;
using AutoMapper;

namespace VolunteerHub.Backend.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
           CreateMap<User,RegisterDto>().ReverseMap();
           CreateMap<ProjectTask, ProjectTasksDto>().ReverseMap();
        }
    }
}
