using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VolunteerHub.Backend.Helpers;
using VolunteerHub.Backend.Models;
using VolunteerHub.Backend.Services.Interfaces;
using VolunteerHub.DataAccessLayer.Interfaces;
using VolunteerHub.DataModels.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VolunteerHub.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnouncementsController : ControllerBase
    {
        private readonly IAnnouncementRepository _announcementRepository;
        private readonly IEmailService _emailService;
        private readonly JwtService _jwtService;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly VolunteerHubContext _context;
        private readonly IProjectRepository _projectRepository;
        private readonly IUserOrganizationRepository _userOrganizationRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IUserTaskRepository _userTaskRepository;

        public AnnouncementsController(IAnnouncementRepository announcementRepository,
            IEmailService emailService,
            JwtService jwtService,
            IOrganizationRepository organizationRepository,
            UserManager<User> userManager,
            IMapper mapper,
            VolunteerHubContext context,
            IProjectRepository projectRepository,
            IUserOrganizationRepository userOrganizationRepository,
            ITaskRepository taskRepository,
            IUserTaskRepository userTaskRepository
            
            )
        {
            _announcementRepository = announcementRepository;
            _emailService = emailService;
            _jwtService = jwtService;
            _organizationRepository = organizationRepository;
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
            _projectRepository = projectRepository;
            _userOrganizationRepository = userOrganizationRepository;
            _taskRepository = taskRepository;
            _userTaskRepository = userTaskRepository;
        }

       // [Authorize(Roles = "Admin")]
        [HttpGet("AllAnouncements")]
        public IActionResult GetAllAnnouncements()
        {
            var announcements = _announcementRepository.GetAll();
            return Ok(announcements);
        }

        //[Authorize(Roles ="Admin")]
        //[Authorize(Roles ="Coordinator")]
        [HttpPost("{projectId:long}/createAnnouncement")]
        public IActionResult CreateAnnouncement(long projectId, AnnouncementDto announcementDto)
        {
            var project = _projectRepository.GetById(projectId);
            if(project == null)
            {
                return BadRequest("No project found");
            }
            var organizationId = project.OrganizationId;

            var organization = _organizationRepository.GetById(organizationId);
            if(organization == null)
            {
                return BadRequest("No organization Found");
            }
           
            var loggedUser = _userManager.GetUserAsync(User);
            if(loggedUser.Result == null)
            {
                return BadRequest("User not found");
            }
            
            if(loggedUser.Result.Id != organization.OwnerId) {
                return Forbid("You don't own this organization");
            }
            //pana aici e autorizarea

            var announcement = new Announcement();
            announcement.CreatedAt = DateTime.Now;
            announcement.UpdatedAt = DateTime.Now;
            announcement.Title = announcementDto.Title;
            announcement.ProjectId = projectId;
            announcement.Project = project;
            announcement.Content = announcementDto.Content;
            _announcementRepository.Add(announcement);
            _announcementRepository.Save();
            /*var tasks = _taskRepository.Get(t => t.ProjectId == announcement.ProjectId);
            var userEmails = new List<string>();
            foreach ( var task in tasks)
            {
                var users = _userTaskRepository.Get(ut => ut.TaskId == task.Id);
                if(users.Count == 0)
                {
                    return BadRequest("No users found for this task");
                }
                foreach ( var user in users)
                {
                    if( user.UserId == null)
                    {
                        continue;
                    }
                    var u = _userManager.FindByIdAsync(user.UserId);
                    if(u.Result == null)
                    {
                        continue;
                    }
                    if (u.Result.Email == null)
                    {
                        continue;
                    }
                    userEmails.Add(u.Result.Email);
                }
                foreach( var userEmail in userEmails.Distinct())
                {
                    Task.Run(() => _emailService.SendEmailAsync(userEmail, announcement.Title, announcement.Content).Wait());
                }
             } */
            return Ok("Success");
        }

        [HttpGet("{Id}/announcements")]
        public IActionResult GetAnnouncement(long Id) {

            /*
            IList<Announcement> announcement;
            var org = _organizationRepository.GetById(Id);
            var projects = _projectRepository.Get(p => p.OrganizationId == Id);
            List <Announcement> announcements = new();
            foreach(var proj in projects)
            {
                var ann = _announcementRepository.Get(a => a.ProjectId == proj.Id);
                foreach(var a in ann)
                {
                    announcements.Add(a);
                }
            }
                */
            var announcements = _announcementRepository.GetAll();
            return Ok(announcements);
        }

        [HttpPost("{Id:long}/deleteAnnouncement")]
        public IActionResult deleteAnnouncement( long Id)
        {
            
            var announcement = _announcementRepository.GetById(Id);


            if(announcement == null)
            {
                return BadRequest("No announcement found !");
            }

            var project = _projectRepository.GetById(announcement.ProjectId);
            if (project == null)
            {
                return BadRequest("No project found");
            }
            var organizationId = project.OrganizationId;

            var organization = _organizationRepository.GetById(organizationId);
            if (organization == null)
            {
                return BadRequest("No organization Found");
            }
            if (User.Identity == null)
            {
                return BadRequest("Not logged in");
            }
            var username = User.Identity.Name;
            if (username == null)
            {
                return BadRequest("Username not found");
            }
            var loggedUser = _userManager.FindByNameAsync(username);
            if (loggedUser.Result == null)
            {
                return BadRequest("User not found");
            }

            if (loggedUser.Result.Id != organization.OwnerId || !_userManager.IsInRoleAsync(loggedUser.Result, "Admin").Result)
            {
                return Forbid("You don't own this organization");
            }


            _announcementRepository.Delete(announcement);
            _announcementRepository.Save();
            return Ok();
        }
    }
}
