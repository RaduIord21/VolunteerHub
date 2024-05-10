using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VolunteerHub.DataAccessLayer.Interfaces;
using VolunteerHub.DataModels.Models;

namespace VolunteerHub.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserStatsController : ControllerBase
    {

   
        private readonly IUserStatsRepository _userStatsRepository;

        public UserStatsController(
               IUserStatsRepository userStatsRepository
            )
        {
            _userStatsRepository = userStatsRepository;
        }

        [Authorize]
        [HttpGet("{userId}/UserStats")]
        public IActionResult GetUserStats([FromRoute(Name = "userId")] string? uid)
        {
            if (uid == null)
            {
                return BadRequest("Got null value from route");
            }
            var userStats = _userStatsRepository.GetByUserId(uid);
            if (userStats == null)
            {
                return BadRequest("No stats found for user : ");
            }
            return Ok(userStats);
        }
    }
}
