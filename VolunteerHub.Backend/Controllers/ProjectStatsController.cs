﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VolunteerHub.DataAccessLayer.Interfaces;
using VolunteerHub.DataAccessLayer.Repositories;
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

        [Authorize(Roles = "Admin")]
        [HttpGet("AllProjectStats")]
        public IActionResult AllProjectStats()
        {
            return Ok(_projectStatsRepository.GetAll());
        }


        [Authorize]
        [HttpGet("{Id:long}/ProjectStats")]
        public IActionResult GetStats(long Id)
        {
            var stats = _projectStatsRepository.GetByProjectId(Id); 
            return Ok(stats);
        }
    }
}
