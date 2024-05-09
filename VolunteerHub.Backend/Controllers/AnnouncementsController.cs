using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        private readonly ITaskRepository _taskRepository;
        private readonly IUserTaskRepository _userTaskRepository;
        private readonly UserManager<User> _userManager;

        public AnnouncementsController(IAnnouncementRepository announcementRepository,
            IEmailService emailService,
            ITaskRepository taskRepository,
            IUserTaskRepository userTaskRepository,
            UserManager<User> userManager
            )
        {
            _announcementRepository = announcementRepository;
            _emailService = emailService;
            _taskRepository = taskRepository;
            _userTaskRepository = userTaskRepository;
            _userManager = userManager;
        }

        [HttpPost("{projectId:long}/createAnnouncement")]
        public IActionResult CreateAnnouncement(long projectId, AnnouncementDto announcementDto)
        {
            var announcement = new Announcement();
            announcement.CreatedAt = DateTime.Now;
            announcement.UpdatedAt = DateTime.Now;
            announcement.Title = announcementDto.Title;
            announcement.ProjectId = projectId;
            announcement.Content = announcementDto.Content;
            _announcementRepository.Add(announcement);
            _announcementRepository.Save();
            var tasks = _taskRepository.Get(t => t.ProjectId == announcement.ProjectId);
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
             }
            return Ok("Success");
        }

        [HttpGet("{Id}/announcements")]
        public IActionResult GetAnnouncement(long Id) {

            IList<Announcement> announcement ;
            
            announcement = _announcementRepository.Get(a => a.ProjectId == Id);
                
            return Ok(announcement);
        }

        [HttpPost("{Id:long}/deleteAnnouncement")]
        public IActionResult deleteAnnouncement( long Id)
        {
            var announcement = _announcementRepository.GetById(Id);
            if(announcement == null)
            {
                return BadRequest("No announcement found !");
            }
            _announcementRepository.Delete(announcement);
            _announcementRepository.Save();
            return Ok();
        }
    }
}
