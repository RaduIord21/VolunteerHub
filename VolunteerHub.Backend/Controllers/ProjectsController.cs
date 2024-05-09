﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using VolunteerHub.Backend.Helpers;
using VolunteerHub.Backend.Models;
using VolunteerHub.DataAccessLayer.Interfaces;
using VolunteerHub.DataModels.Models;

namespace VolunteerHub.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly JwtService _jwtService;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IProjectStatsRepository _projectStatsRepository;

        public ProjectsController(
            IProjectRepository projectRepository,
            IMapper mapper,
            UserManager<User> userManager,
            JwtService jwtService,
            IOrganizationRepository organizationRepository,
            IProjectStatsRepository projectStatsRepository
            )
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
            _userManager = userManager;
            _jwtService = jwtService;
            _organizationRepository = organizationRepository;
            _projectStatsRepository = projectStatsRepository;
        }

        [AllowAnonymous]
        [HttpGet("{Id:long}/getProject")]
        public IActionResult GetProject(long Id)
        {
           
            var project = _projectRepository.GetById(Id);
            
            if (project == null)
            {
                return BadRequest();
            }
            return Ok(project);
        }
        [AllowAnonymous]
        [HttpGet("{Id:long}/projects")]
        public IActionResult Projects(long Id)
        {
            try
            {
                var organization = _organizationRepository.GetById(Id);  
                if (organization == null)
                {
                    return BadRequest("Organization not found");
                }
                var projects = _projectRepository.Get(p => p.OrganizationId == organization.Id);
                return Ok(projects);
            }
            catch (Exception e)
            {
                return BadRequest("Backend Error " + e.Message);
            }
        }
        [HttpPost("createProject")]
        public IActionResult createProject(ProjectsDto projectsDto)
        {
            try
            {
                if(projectsDto.OrganizationId == null)
                {
                    return BadRequest("No organization Found");
                }
                if (User.Identity == null)
                {
                    return BadRequest("Null identity");
                }
                var userName = User.Identity.Name;
                if (userName == null)
                {
                    return Ok("Identity not found");
                }
                var user = _userManager.FindByNameAsync(userName);
                if (user.Result == null)
                {
                    return Ok("User not found");
                }
                

                var project = new Project
                {
                    Organization = _organizationRepository.GetById(projectsDto.OrganizationId),
                    OrganizationId = (long)projectsDto.OrganizationId,
                    ProjectName = projectsDto.ProjectName,
                    Description = projectsDto.Description,
                    EndDate = projectsDto.EndDate,
                    OwnerId = user.Result.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _projectRepository.Add(project);
                _projectRepository.Save();

                user.Result.ProjectId = project.Id;
                _ = _userManager.UpdateAsync(user.Result);
                var projectStat = new ProjectStat { 
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ProjectId = project.Id,
                    TotalTasksAsigned = 0,
                    TotalTasksCompleted = 0,
                    TotalTasksUncompleted = 0
                };
                _projectStatsRepository.Add(projectStat);
                _projectStatsRepository.Save();
                

                return Ok("Success");
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
        [Authorize(Roles ="Coordinator,Admin")]
        [HttpPost("{Id:long}/changeProjectName")]
        public IActionResult changeProjectName(long Id, [FromBody] ChangeProjectNameDto changeProjectNameDto)
        {
            try
            {
                if(changeProjectNameDto == null)
                {
                    return BadRequest("No data recieved");
                }
                var project = _projectRepository.GetById(Id);
                if (project == null)
                {
                    return BadRequest("No project found");
                }   
                if(changeProjectNameDto.Name.IsNullOrEmpty())
                {
                    return BadRequest("Data is missing");
                }
                project.ProjectName = changeProjectNameDto.Name;
                _projectRepository.Update(project);
                _projectRepository.Save();
                return Ok("Names Changed sucessfully");
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

       

        [HttpPost("{Id:long}/changeDescription")]
        public IActionResult ChangeDescription(long Id,[FromBody]ChangeDescriptionDto changeDescriptionDto)
        
        {
            if (changeDescriptionDto == null)
            {
                return BadRequest("Server got null");
            }
            var project = _projectRepository.GetById(Id);
            if (project == null)
            {
                return BadRequest("No project found");
            }

            if(changeDescriptionDto.Description == null)
            {
                return BadRequest("No description");
            }
            project.Description = changeDescriptionDto.Description;
            _projectRepository.Update(project);
            _projectRepository.Save();
            return Ok("Description Changed sucessfully");
        }


        [HttpPost("{Id:long}/addMembers")]
        public IActionResult AddMembers(long Id,[FromBody]ProjectUserDto projectUsers) 
        {
            if (projectUsers.UserIds == null)
            {
                return BadRequest("No users found");
            }
            foreach(var Uid in projectUsers.UserIds)
            {
                var u = _userManager.FindByIdAsync(Uid);
                if(u.Result == null)
                {
                    continue;
                }
                u.Result.ProjectId = Id;
                var _ = _userManager.UpdateAsync(u.Result).Result;
            }
            return Ok("Success");
        }

       [HttpPost("{Id:long}/deleteProject")]
        public IActionResult deleteProject(long Id)
        {
            var project = _projectRepository.GetById(Id);
            if (project == null)
            {
                return BadRequest("Project Not found");
            }
            _projectRepository.Delete(project);
            _projectRepository.Save();
            var stats = _projectStatsRepository.GetByProjectId(project.Id);
            if(stats == null)
            {
                return BadRequest("Stats not found for this project");
            }
            _projectStatsRepository.Delete(stats);
            _projectStatsRepository.Save();
            return Ok("Success");
        }

        
    }
}
