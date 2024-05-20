﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using VolunteerHub.Backend.Helpers;
using VolunteerHub.Backend.Models;
using VolunteerHub.Backend.Services.Interfaces;
using VolunteerHub.DataAccessLayer.Interfaces;
using VolunteerHub.DataModels.Models;

namespace VolunteerHub.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : Controller
    {
        private readonly IOrganizationRepository _organizationRepository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly VolunteerHubContext _context;
        private readonly IProjectRepository _projectRepository;
        private readonly IUserOrganizationRepository _userOrganizationRepository;
        private readonly IEmailService _emailService;

        public OrganizationController(

            IOrganizationRepository organizationRepository,
            UserManager<User> userManager,
            IMapper mapper,
            VolunteerHubContext context,
            IProjectRepository projectRepository,
            IUserOrganizationRepository userOrganizationRepository,
            IEmailService emailService
            )
        {
            _organizationRepository = organizationRepository;
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
            _projectRepository = projectRepository;
            _userOrganizationRepository = userOrganizationRepository;
            _emailService = emailService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("AllOrganizations")]
        public IActionResult GetAllOrganizations()
        {
            return Ok(_organizationRepository.GetAll());
        }

        [Authorize]
        [HttpPost("{id:long}/invite")]
        public async Task<IActionResult> SendInvitation(long id, InvitationDto invitationDto)
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null) {
                return BadRequest("User not found");
            }
            var organization = _organizationRepository.GetById(id);
            if(organization == null)
            {
                return BadRequest("No organization found");
            }
            if(invitationDto.email == null) {
                return BadRequest("No email received");
            }

            var emailAtribute = new EmailAddressAttribute();
            if (!emailAtribute.IsValid(invitationDto.email))
            {
                return BadRequest("Email is invaild");
            }
            if(organization.OwnerId != user.Id)
            {
                return BadRequest("You are not allowed to invite people to the organization");
            }
            await _emailService.SendEmailAsync(invitationDto.email, $"Invitation for {organization.Name}", $"Acesta este codul de acces pentru organizatia {organization.Name} \n" +
                $"Cod: {organization.Code}");
            return Ok("Succes");
        }

        [Authorize]
        [HttpPost("joinOrganization")]
        public IActionResult JoinOrganization([FromQuery(Name = "Code")] string code)
        {
            try
            {
                var org = _organizationRepository.GetByCode(code);
                if (org == null)
                {
                    return Ok("No organization found");
                }

                
                var user = _userManager.GetUserAsync(User);
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
        [AllowAnonymous]
        [HttpPost("createOrganization")]
        public IActionResult CreateOrganization([FromBody] OrganizationDto organizationDto)
        {
            try
            {

                
                var user = _userManager.GetUserAsync(User);
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

        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Coordinator")]

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

            //verificarea pentru admin

            var isAdmin = _userManager.IsInRoleAsync(user.Result, Constants.AdministratorRole);

            if (isAdmin.Result)
            {
                _organizationRepository.Delete(org);
                _organizationRepository.Save();
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
        public IActionResult QuitOrganization(long Id, quitOrganizationDto quitOrganizationDto)
        {
            var user = _userManager.GetUserAsync(User);

            if (user.Result == null)
            {
                return BadRequest("No user found");
            }
            var org = _organizationRepository.GetById(Id);
            if(org == null)
            {
                return BadRequest("No organization found");
            }
            if(org.OwnerId != user.Result.Id)
            {
                var userOrgs = _userOrganizationRepository.Get(uo => uo.UserId == user.Result.Id && uo.OrganizationId == org.Id);
                foreach(var uo in userOrgs)
                {
                    _userOrganizationRepository.Delete(uo);
                    _userOrganizationRepository.Save();
                }
                return Ok("User removed from organization");
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

        [Authorize]
        [HttpGet("{Id:long}/organization")]
        public IActionResult Organization(long Id)
        {
            var Organization = _organizationRepository.GetById(Id);
            if (Organization == null)
            {
                return BadRequest("No Organization Found");
            }
            
            return Ok(Organization);
        }

        [Authorize]
        [HttpGet("{Id:long}/organization-users")]
        public IActionResult GetOrganizationUsers(long Id)
        {
            var Organization = _organizationRepository.GetById(Id);
            if (Organization == null)
            {
                return BadRequest("No Organization Found");
            }
            var userOrgs = _userOrganizationRepository.Get(uo => uo.OrganizationId == Organization.Id);
            var response = new List<object>();
            foreach (var uo in userOrgs)
            {
                if (uo.UserId == null)
                {
                    continue;
                }
                var u = _userManager.FindByIdAsync(uo.UserId);
                if (u.Result == null)
                {
                    continue;
                }
                var currentObject = new { 
                    u.Result.Email,
                    u.Result.UserName,
                    roles = _userManager.GetRolesAsync(u.Result).Result
                };
                response.Add(currentObject);
            }

            return Ok(response);
        }


        [Authorize]
        [HttpGet("organizations")]
        public IActionResult Organizations()
        {
            try
            {
                var user = _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    return BadRequest("Not logged in");
                }

                var UserOrgs = _userOrganizationRepository.Get(o => o.User == user.Result);
                if (UserOrgs == null)
                {
                    return Ok("No organizations found for the user");
                }

                List<object> response = new();
                foreach (var uo in UserOrgs)
                {
                    var o = _organizationRepository.GetById(uo.OrganizationId);
                    if (o == null)
                    {
                        continue;
                    }
                    var isOwner = false;
                    if(o.OwnerId == user.Result.Id)
                    {
                        isOwner = true;
                    }
                    var rsp = new
                    {
                        OrganizationId = o.Id,
                        OrganizationName = o.Name,
                        IsOwner = isOwner
                    };
                    response.Add(rsp);
                }
                
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Coordinator")]
        [HttpPost("{UserId}/kick")]
        public IActionResult KickMember(string UserId, KickDto kickDto)
        {

            var user = _userManager.FindByIdAsync(UserId);
            if (user.Result == null)
            {
                return BadRequest("User Not vound");
            }
            var organization = _organizationRepository.GetById(kickDto.OrganizationId);
            if (organization == null)
            {
                return BadRequest("No organization found");
            }

            if (User.Identity == null)
            {
                return BadRequest("No user found");
            }

            if (User.Identity.Name == null)
            {
                return BadRequest("No user found");
            }

            var loggedUser = _userManager.FindByNameAsync(User.Identity.Name);

            if (loggedUser.Result == null)
            {
                return BadRequest("No user found");
            }

            if (organization.OwnerId != loggedUser.Result.Id)
            {
                return Forbid("Not an owner of this organization");
            }

            var userOrganizations = _userOrganizationRepository.Get(uo => uo.UserId == UserId && uo.OrganizationId == kickDto.OrganizationId);
            _userOrganizationRepository.Delete(userOrganizations[0]);
            _userOrganizationRepository.Save();
            return Ok("Successfully kicked player");
        }
    }

}
