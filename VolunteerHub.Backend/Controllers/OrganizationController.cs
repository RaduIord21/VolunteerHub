using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.CodeDom.Compiler;
using VolunteerHub.Backend.Helpers;
using VolunteerHub.Backend.Models;
using VolunteerHub.DataAccessLayer.Interfaces;
using VolunteerHub.DataModels.Models;

namespace VolunteerHub.Backend.Controllers
{
    [Route("api")]
    [ApiController]
    public class OrganizationController : Controller
    {
        private readonly JwtService _jwtService;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly VolunteerHubContext _context;


        public OrganizationController(

            JwtService jwtService,
            IOrganizationRepository organizationRepository,
            UserManager<User> userManager,
            IMapper mapper,
            VolunteerHubContext context

            )
        {
            _jwtService = jwtService;
            _organizationRepository = organizationRepository;
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
        }

        [HttpPost("createOrganization")]
        public IActionResult CreateOrganization([FromBody] OrganizationDto organizationDto)
        {
            try
            {
                string code = generateCode();
                organizationDto.Code = code;
                _organizationRepository.Add(_mapper.Map<Organization>(organizationDto));
                _organizationRepository.Save();
                var userName = User.Identity.Name;
                if (userName == null)
                {
                    return Ok("Identitate negasita");
                }
                var user = _userManager.FindByNameAsync(userName).Result;
                if (user == null)
                {
                    return Ok("User negasit in baza de date");
                }
                var Organizations = _organizationRepository.Get(o => o.Name == organizationDto.Name);
                if (Organizations == null)
                {
                    return Ok("Nu s-a gasit organizatie");
                }
                foreach (var org in Organizations)
                {
                    user.Organization = org;
                    _context.Users.Update(user);
                    _context.SaveChanges();
                    return Ok("Succes");
                }
                return Ok("Succes");
            }
            catch (Exception ex)
            {
                return Ok("Eroare la adaugarea organizatiei" + ex.Message);
            }
        }

        private string generateCode()
        {
            Random rnd = new Random();
            string chars = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM0123456789";

            string res = "";
            for (int i = 0; i < 10; i++)
            {
                res += chars[rnd.Next(62)];
            }
            return res;
        }

        [AllowAnonymous]
        [HttpGet("organization")]
        public IActionResult Organizations()
        {
            try
            {
                var jwt = Request.Cookies["jwt"];
                if (jwt == null)
                {
                    return Ok("Not Logged in");
                }
                var token = _jwtService.Verify(jwt);
                var issuer = token.Issuer;
                var user = _userManager.FindByIdAsync(issuer);
                if (user == null)
                {
                    return Ok("No users");
                }
                var organization = _organizationRepository.GetById(user.Result.OrganizationId);
                {
                    if (organization == null) {
                        return Ok();
                    }
                    return Ok(organization.Name);
                }
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }



    }

}
