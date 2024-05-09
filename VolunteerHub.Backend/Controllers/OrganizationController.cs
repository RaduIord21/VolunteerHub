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
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : Controller
    {
        private readonly JwtService _jwtService;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly VolunteerHubContext _context;
        private readonly IProjectRepository _projectRepository;
        private readonly IUserOrganizationRepository _userOrganizationRepository;

        public OrganizationController(

            JwtService jwtService,
            IOrganizationRepository organizationRepository,
            UserManager<User> userManager,
            IMapper mapper,
            VolunteerHubContext context,
            IProjectRepository projectRepository,
            IUserOrganizationRepository userOrganizationRepository
            )
        {
            _jwtService = jwtService;
            _organizationRepository = organizationRepository;
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
            _projectRepository = projectRepository;
            _userOrganizationRepository = userOrganizationRepository;
        }
        [HttpPost("joinOrganization")]
        public IActionResult JoinOrganization([FromQuery(Name ="Code")]string code)
        {
            try
            {
                var org = _organizationRepository.GetByCode(code);
                if (org == null)
                {
                    return Ok("No organization found");
                }
                
                if (User.Identity.Name == null)
                {
                    return BadRequest("Not logged in");
                }
                var user = _userManager.FindByNameAsync(User.Identity.Name);
                if (user.Result == null)
                {
                    return BadRequest("No user found");
                }
                UserOrganization uo = new()
                {
                    UserId = user.Result.Id,
                    OrganizationId = org.Id,
                };
                _userOrganizationRepository.Add(uo);
                _userOrganizationRepository.Save();
                var rez = new
                {
                    organizationName = org.Name,
                    organizationContact = org.Contact,
                    organizationAdress = org.Adress,
                    organizationCode = org.Code
                };
                return Ok(rez);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("createOrganization")]
        public IActionResult CreateOrganization([FromBody] OrganizationDto organizationDto)
        {
            try
            {

                if (User.Identity == null)
                {
                    return BadRequest("missing Identity");
                }
                var userName = User.Identity.Name;
                if (userName == null)
                {
                    return Ok("Identitate negasita");
                }
                var user = _userManager.FindByNameAsync(userName);
                if (user.Result == null)
                {
                    return Ok("User negasit in baza de date");
                }

                string code = generateCode();
                organizationDto.Code = code;
                Organization org = new()
                {
                    Name = organizationDto.Name,
                    Contact = organizationDto.Contact,
                    Adress = organizationDto.Adress,
                    Code = code,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    OwnerId = user.Result.Id
                };
                _organizationRepository.Add(org);
                _organizationRepository.Save();
                Task.Run(() => _userManager.RemoveFromRoleAsync(user.Result, Constants.VolunteerRole)).Wait();
                Task.Run(() => _userManager.AddToRoleAsync(user.Result, Constants.CoordinatorRole)).Wait();

                
                var UserOrg = new UserOrganization()
                {
                    OrganizationId = org.Id,
                    UserId = user.Result.Id,
                };
                _userOrganizationRepository.Add(UserOrg);
                _userOrganizationRepository.Save();
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return Ok("Eroare la adaugarea organizatiei" + ex.Message);
            }
        }

        private static string generateCode()
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

        [HttpPost("{Id:long}/deleteOrganization")]
        public IActionResult DeleteOrganization(long Id)
        {
            if (User.Identity == null)
            {
                return BadRequest("No user found");
            }

            if (User.Identity.Name == null)
            {
                return BadRequest("No user found");
            }

            var user = _userManager.FindByNameAsync(User.Identity.Name);
            if (user.Result == null)
            {
                return BadRequest("No user found");
            }
            var org = _organizationRepository.GetById(Id);
            if (org == null)
            {
                return BadRequest("No Organization");
            }
            var orgs = _organizationRepository.Get(o => o.OwnerId == user.Result.Id);
            if (orgs.Count == 0)
            {
                Task.Run(() => _userManager.RemoveFromRoleAsync(user.Result, Constants.CoordinatorRole)).Wait();
                Task.Run(() => _userManager.AddToRoleAsync(user.Result, Constants.VolunteerRole)).Wait();
            }
            _organizationRepository.Delete(org);
            _organizationRepository.Save();
            return Ok("Success");
        }

        [HttpPost("{Id:long}/quitOrganization")]
        public IActionResult QuitOrganization(long Id,quitOrganizationDto quitOrganizationDto)
        {

            if (User.Identity == null)
            {
                return BadRequest("No user found");
            }

            if (User.Identity.Name == null)
            {
                return BadRequest("No user found");
            }

            var user = _userManager.FindByNameAsync(User.Identity.Name);

            if (user.Result == null)
            {
                return BadRequest("No user found");
            }

            Task.Run(() => _userManager.RemoveFromRoleAsync(user.Result, Constants.CoordinatorRole)).Wait();
            Task.Run(() => _userManager.AddToRoleAsync(user.Result, Constants.VolunteerRole)).Wait();
            
            var userOrganization = _userOrganizationRepository.Get(uo => uo.OrganizationId == Id && uo.UserId == user.Result.Id);
            if (userOrganization == null)
            {
                return BadRequest("User not in the organization");
            }
            _userOrganizationRepository.Delete(userOrganization[0]);
            _userOrganizationRepository.Save();
            if (quitOrganizationDto.NewCoordinatorId == null)
            {
                return BadRequest("No user found");
            }

            var newCoord = _userManager.FindByIdAsync(quitOrganizationDto.NewCoordinatorId);

            if (newCoord.Result == null)
            {
                return BadRequest("No user found");
            }

            Task.Run(() => _userManager.RemoveFromRoleAsync(newCoord.Result, Constants.VolunteerRole)).Wait();
            Task.Run(() => _userManager.AddToRoleAsync(newCoord.Result, Constants.CoordinatorRole)).Wait();

            
            return Ok("Success");
        }

        [HttpGet("{Id:long}/organization")]
        public IActionResult Organization(long Id)
        {
            var Organization = _organizationRepository.GetById(Id);
            if (Organization == null) {
                return BadRequest("No Organization Found");
            }
            var userOrgs = _userOrganizationRepository.Get(uo => uo.OrganizationId == Organization.Id);
            IList<User>? Users = new List<User>();
            foreach (var uo in userOrgs) {
                if (uo.UserId == null)
                {
                    continue;
                }
                var u = _userManager.FindByIdAsync(uo.UserId);
                if (u.Result == null) {
                    continue;
                }
                Users.Add(u.Result);
            }
            var rsp = new
            {
                Users,
                Organization
            };
            return Ok(rsp);
        }

        [AllowAnonymous]
        [HttpGet("organizations")]
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
                if (user.Result == null)
                {
                    return Ok("No users");
                }
                var UserOrgs = _userOrganizationRepository.Get(o => o.UserId == user.Result.Id);
                if (UserOrgs == null)
                {
                    return Ok("No organizations found for the user");
                }
                IList<Organization>? orgs = new List<Organization>();
                foreach (var uo in UserOrgs)
                {
                    var o = _organizationRepository.GetById(uo.OrganizationId);
                    if (o == null)
                    {
                        continue;
                    }
                    orgs.Add(o);
                }
                return Ok(orgs);
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        [HttpPost("{UserId}/kick")]
        public IActionResult kickMember (string UserId, KickDto kickDto)
        {
           
            var user = _userManager.FindByIdAsync(UserId);
            if (user.Result == null)
            {
                return BadRequest("User Not vound");
            }
            var userOrganizations  = _userOrganizationRepository.Get(uo => uo.UserId == UserId && uo.OrganizationId == kickDto.OrganizationId);
            _userOrganizationRepository.Delete(userOrganizations[0]);
            _userOrganizationRepository.Save();
            return Ok("Successfully kicked player");
        }
    }

}
