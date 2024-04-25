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

        [HttpGet("ProjectStats/{projectId}")]
        public IActionResult GetStats([FromRoute(Name = "projectId")] string DtoId)
        {
            ProjectStat? stats;
            if (long.TryParse(DtoId, out long Id))
            {
                stats = _projectStatsRepository.GetByProjectId(Id);
                if (stats == null)
                {
                    return BadRequest("Stats not Found");
                }
            }
            else
            {
                return BadRequest("conversion impossible");
            }

            return Ok(stats);
        }
    }
}
