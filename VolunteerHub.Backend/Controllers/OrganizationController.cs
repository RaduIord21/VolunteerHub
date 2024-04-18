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
        private readonly IUserRepository _userRepository;

        public OrganizationController(

            JwtService jwtService,
            IOrganizationRepository organizationRepository,
            UserManager<User> userManager,
            IMapper mapper,
            VolunteerHubContext context,
            IUserRepository userRepository
            )
        {
            _jwtService = jwtService;
            _organizationRepository = organizationRepository;
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
            _userRepository = userRepository;
        }
        [HttpPost("joinOrganization")]
        public IActionResult JoinOrganization([FromBody] JoinOrganizationDto joinOrganizationDto)
        {
            try
            {
                var org = _organizationRepository.Get(o => o.Code == joinOrganizationDto.Code)[0];
                if (org == null)
                {
                    return Ok("No organization found");
                }
                if (User.Identity.Name == null) {
                    return BadRequest("Not logged in");
                }
                var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
                user.OrganizationId = org.Id;
                user.Organization = org;
                _context.Users.Update(user);
                _context.SaveChanges();
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
                return BadRequest("crapa");
            }
        }

        [HttpPost("createOrganization")]
        public IActionResult CreateOrganization([FromBody] OrganizationDto organizationDto)
        {
            try
            {

                if (User.Identity == null) { 
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
                if (user.Result.OrganizationId != null)
                {
                    return BadRequest("Deja exista organizatie");
                }
                string code = generateCode();
                organizationDto.Code = code;
                _organizationRepository.Add(_mapper.Map<Organization>(organizationDto));
                _organizationRepository.Save();
                Task.Run(() => _userManager.RemoveFromRoleAsync(user.Result, Constants.VolunteerRole)).Wait();
                Task.Run(() => _userManager.AddToRoleAsync(user.Result, Constants.CoordinatorRole)).Wait();
                var Organizations = _organizationRepository.Get(o => o.Name == organizationDto.Name);
                if (Organizations == null)
                {
                    return Ok("Nu s-a gasit organizatie");
                }
                foreach (var org in Organizations)
                {
                    user.Result.Organization = org;
                    user.Result.OrganizationId = org.Id;
                    _context.Users.Update(user.Result);
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

        [HttpPost("quitOrganization")]
        public IActionResult quitOrganization(quitOrganizationDto quitOrganizationDto)
        {
            string? response = quitOrganizationDto.User;
            if (string.IsNullOrEmpty(response))
            { 
                return BadRequest("No user");
            }
            var user = _userManager.FindByNameAsync(response);
            if (user.Result == null)
            {
                return BadRequest("User not Found");
            }
                      if (user.Result.OrganizationId == null)
            {
                return BadRequest("Backend : Nu e organizatie");
            }
            if (_userManager.IsInRoleAsync(user.Result, Constants.CoordinatorRole).Result)
            {
                var org = _organizationRepository.GetById(user.Result.OrganizationId);
                if (org == null)
                {
                    return BadRequest("No organization ");
                }
                _organizationRepository.Delete(org);
                _organizationRepository.Save();
                Task.Run(() => _userManager.RemoveFromRoleAsync(user.Result, Constants.CoordinatorRole));
                Task.Run(() => _userManager.AddToRoleAsync(user.Result, Constants.VolunteerRole));
            }
            else
            {
                user.Result.OrganizationId = null;
                user.Result.Organization = null;
                _context.Users.Update(user.Result);
                _context.SaveChanges();
            }
            return Ok("Succcesfully Removed");
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
                if (user.Result == null)
                {
                    return Ok("No users");
                }
                var organization = _organizationRepository.GetById(user.Result.OrganizationId);
                {
                    if (organization == null)
                    {
                        return Ok();
                    }

                    var users = _context.Users.Where(u => u.OrganizationId == organization.Id).ToList();
                    //var users = _userRepository.Get(u => u.OrganizationId == organization.Id).ToList();
                    var usersList = new List<object>();
                    foreach (var u in users) 
                    {
                        var curr = new
                        {
                            u.Id,
                            u.UserName,
                            u.Email,
                            _userManager.GetRolesAsync(u).Result
                        };
                        usersList.Add(curr);
                    }
                    //Console.WriteLine(users);
                    var org = new
                    {
                
                        organizationName = organization.Name,
                        organizationContact = organization.Contact,
                        organizationAdress = organization.Adress,
                        organizationCode = organization.Code,
                        currentUser = user.Result.UserName,
                        currentEmail = user.Result.Email,
                        currentRole = _userManager.GetRolesAsync(user.Result).Result[0],
                        users = usersList
                    };
                    return Ok(org);
                }
            }
            catch (Exception e)
            {
                return Ok(e);   
            }
        }

        [HttpPost("kick")]
        public IActionResult kickMember(KickDto kickDto)
        {
            if (kickDto.email == null)
            {
                return BadRequest("No email from view component !");
            }
            var user = _userManager.FindByEmailAsync(kickDto.email);
            if (user.Result == null) {
                return BadRequest("User Not vound");
            }
            user.Result.Organization = null;
            user.Result.OrganizationId = null;
            _context.Users.Update(user.Result);
            _context.SaveChanges();
            return Ok("Successfully kicked player");
        }
    }

}
