using AutoMapper;
using VolunteerHub.Models;
using VolunteerHub.DataModels.Models;
using System.Numerics;


namespace VolunteerHub.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserViewModel>().ReverseMap();
            CreateMap<Role, RoleViewModel>().ReverseMap();
            CreateMap<UserRole, UserRoleViewModel>().ReverseMap();


        }
    }
}
