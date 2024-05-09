using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VolunteerHub.DataAccessLayer.Interfaces;
using VolunteerHub.DataModels.Models;

namespace VolunteerHub.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectStatsController : ControllerBase
    {
        private readonly IProjectStatsRepository _projectStatsRepository;

        public ProjectStatsController(IProjectStatsRepository projectStatsRepository)
        {
            _projectStatsRepository = projectStatsRepository;
        }

        [HttpGet("{Id:long}/ProjectStats")]
        public IActionResult GetStats([FromRoute(Name = "projectId")] long Id)
        {
            var stats = _projectStatsRepository.GetByProjectId(Id);
            return Ok(stats);
        }
    }
}
