using Microsoft.AspNetCore.Mvc;
using VolunteerHub.Backend.Models;
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

        public AnnouncementsController(IAnnouncementRepository announcementRepository)
        {
            _announcementRepository = announcementRepository;
        }

        [HttpPost("createAnnouncement")]
        public IActionResult CreateAnnouncement(AnnouncementDto announcementDto)
        {
            var announcement = new Announcement();
            announcement.CreatedAt = DateTime.Now;
            announcement.UpdatedAt = DateTime.Now;
            announcement.Title = announcementDto.Title;
            announcement.ProjectId = announcementDto.ProjectId;
            announcement.Content = announcementDto.Content;
            _announcementRepository.Add(announcement);
            _announcementRepository.Save();
            return Ok("Success");
        }

        [HttpGet("announcement/{projectId}")]
        public IActionResult GetAnnouncement([FromRoute(Name ="projectId")] string projectId) {

            IList<Announcement> announcement ;
            if (long.TryParse(projectId, out long Id))
            {
                announcement = _announcementRepository.Get(a => a.ProjectId == Id);
                if (announcement == null)
                {
                    return BadRequest("Announcements not found");
                }
            }
            else
            {
                return BadRequest("conversion impossible");
            }
            return Ok(announcement);
        }

        [HttpPost("deleteAnnouncement")]
        public IActionResult deleteAnnouncement(AnnouncementDto announcementDto)
        {
            var announcement = _announcementRepository.GetById(announcementDto.Id);
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
