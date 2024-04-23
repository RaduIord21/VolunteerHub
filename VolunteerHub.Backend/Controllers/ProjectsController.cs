using AutoMapper;
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
    [Route("api")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly JwtService _jwtService;
        private readonly IOrganizationRepository _organizationRepository;

        public ProjectsController(
            IProjectRepository projectRepository,
            IMapper mapper,
            UserManager<User> userManager,
            JwtService jwtService,
            IOrganizationRepository organizationRepository
            )
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
            _userManager = userManager;
            _jwtService = jwtService;
            _organizationRepository = organizationRepository;
        }

        [AllowAnonymous]
        [HttpGet("getProject/{projectId}")]
        public IActionResult GetProject([FromRoute(Name = "projectId")]string DtoId)
        {
            long Id;
            Project? project;
                if (long.TryParse(DtoId, out Id))
            {
                project = _projectRepository.GetById(Id);
            }
            else
            {
                return BadRequest("conversion impossible");
            }
            if (project == null)
            {
                return BadRequest();
            }
            return Ok(project);
        }
        [AllowAnonymous]
        [HttpGet("projects")]
        public IActionResult Projects()
        {
            try
            {
                if (User.Identity == null)
                {
                    return BadRequest("Null identity");
                }
                var userName = User.Identity.Name;
                if (userName == null)
                {
                    return Ok("Identitate negasita");
                }
                var user = _userManager.FindByNameAsync(userName);
                if (user.Result == null)
                {
                    return Ok("No users");
                }
                var organization = _organizationRepository.GetById(user.Result.OrganizationId);
                if (organization == null)
                {
                    return BadRequest("Organization not found for the current user");
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
                if (User.Identity == null)
                {
                    return BadRequest("Null identity");
                }
                var userName = User.Identity.Name;
                if (userName == null)
                {
                    return Ok("Identitate negasita");
                }
                var user = _userManager.FindByNameAsync(userName);
                if (user.Result == null)
                {
                    return Ok("User negasit in baza de date");
                }
                if (user.Result.OrganizationId == null)
                {
                    return BadRequest("No organization found");
                }

                var project = new Project
                {
                   
                    Organization = user.Result.Organization,
                    OrganizationId = (long)user.Result.OrganizationId,
                    ProjectName = projectsDto.ProjectName,
                    Description = projectsDto.Description,
                    EndDate = projectsDto.EndDate,
                    OwnerId = user.Result.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                /*IList<Project> verifyOrg = _projectRepository.Get(p => p.ProjectName == project.ProjectName);
                if (verifyOrg.Count != 0)
                {
                    return BadRequest("An organization with this name already exists");
                }*/
                _projectRepository.Add(project);
                _projectRepository.Save();
                
                return Ok("Success");
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }
        [Authorize(Roles ="Coordinator,Admin")]
        [HttpPost("changeProjectName")]
        public IActionResult changeProjectName([FromBody] ChangeProjectNameDto changeProjectNameDto)
        {
            try
            {
                if(changeProjectNameDto == null)
                {
                    return BadRequest("No data recieved");
                }
                var project = _projectRepository.GetById(changeProjectNameDto.Id);
                if (project == null)
                {
                    return BadRequest("No project found");
                }   
                if(changeProjectNameDto.Name.IsNullOrEmpty())
                {
                    return BadRequest("Data is empty");
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

       

        [HttpPost("changeDescription")]
        public IActionResult ChangeDescription([FromBody]ChangeDescriptionDto changeDescriptionDto)
        
        {
            if (changeDescriptionDto == null)
            {
                return BadRequest("Server got null");
            }
            var project = _projectRepository.GetById(changeDescriptionDto.Id);
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

        [HttpPost("deleteProject")]
        public IActionResult deleteProject(DeleteProjectDto deleteProjectDto)
        {
            var project = _projectRepository.GetById(deleteProjectDto.Id);
            if (project == null)
            {
                return BadRequest("Project Not found");
            }
            _projectRepository.Delete(project);
            _projectRepository.Save();
            return Ok("Success");
        }
    }
}
