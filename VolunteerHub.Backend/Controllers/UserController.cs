using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VolunteerHub.DataAccessLayer.Interfaces;
using VolunteerHub.DataModels.Models;

namespace VolunteerHub.Backend.Controllers
{
    [Route("api")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IOrganizationRepository _organizationRepository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;


        public UserController(UserManager<User> userManager,
               IOrganizationRepository organizationRepository,
               IMapper mapper
            )
        {
            _userManager = userManager;
            _organizationRepository = organizationRepository;
            _mapper = mapper;
        }



    }
}
