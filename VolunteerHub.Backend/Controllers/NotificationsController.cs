using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using VolunteerHub.Backend.Models;
using VolunteerHub.Backend.Services.Interfaces;
using VolunteerHub.DataAccessLayer.Interfaces;
using VolunteerHub.DataAccessLayer.Repositories;
using VolunteerHub.DataModels.Models;

namespace VolunteerHub.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly INotificationRepository _notificationRepository;
        private readonly UserManager<User> _userManager;
        private readonly IOrganizationRepository _organizationRepository;
        public NotificationsController(
            IEmailService emailSerive,
            INotificationRepository notificationRepository,
            UserManager<User> userManager,
            IOrganizationRepository organizationRepository
            )
        {
            _emailService = emailSerive;
            _notificationRepository = notificationRepository;
            _userManager = userManager;
            _organizationRepository = organizationRepository;
        }

        [HttpGet("Notifications")]
        public IActionResult Notifications() {

            if(User.Identity == null) {
                return BadRequest("Not logged in");
            }
            var username = User.Identity.Name;
            if(username == null )
            {
                return BadRequest("No username found");
            }
            var user = _userManager.FindByNameAsync(username);
            if(user.Result == null)
            {
                return BadRequest("No user found");
            }
            var notifications = _notificationRepository.Get(n => n.Email == user.Result.Email);
            
            return Ok(notifications);
        }

        [HttpPost("{Id:long}/readNotification")]
        public IActionResult readNotification(long Id)
        {
            var notification = _notificationRepository.GetById(Id);
            if (notification == null)
            {
                return BadRequest("Notification not found");
            }
            notification.IsRead = true;
            _notificationRepository.Update(notification);
            _notificationRepository.Save();
            return Ok("Success");
            

        }

        [Authorize(Roles ="Admin")]
        [Authorize(Roles ="Coordinator")]
        [HttpPost("sendNotification")]
        public async Task<IActionResult> SendNotification ([FromBody] NotificationDto notificationDto)
        {

            

            //pana aici e autorizarea
            
            Notification notification = new();
            if (notificationDto.Email == null || notificationDto.Subject == null || notificationDto.Content == null) {
                return BadRequest("Invalid message");
            }
            notification.Subject = notificationDto.Subject;
            notification.Email = notificationDto.Email;
            notification.Content = notificationDto.Content;
            await _emailService.SendEmailAsync(notificationDto.Email, notificationDto.Subject, notificationDto.Content);
            _notificationRepository.Add(notification);
            _notificationRepository.Save();
            return Ok("Sent");
        }
    }
}
