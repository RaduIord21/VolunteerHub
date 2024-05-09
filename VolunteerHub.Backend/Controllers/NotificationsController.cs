using Microsoft.AspNetCore.Mvc;
using VolunteerHub.Backend.Models;
using VolunteerHub.Backend.Services.Interfaces;
using VolunteerHub.DataAccessLayer.Interfaces;
using VolunteerHub.DataModels.Models;

namespace VolunteerHub.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly INotificationRepository _notificationRepository;
        public NotificationsController(IEmailService emailSerive, INotificationRepository notificationRepository)
        {
            _emailService = emailSerive;
            _notificationRepository = notificationRepository;
        }

        [HttpPost("sendNotification")]
        public async Task<IActionResult> SendNotification ([FromBody] NotificationDto notificationDto)
        {
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
