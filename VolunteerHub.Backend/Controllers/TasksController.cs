using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VolunteerHub.Backend.Models;
using VolunteerHub.DataAccessLayer.Interfaces;
using VolunteerHub.DataModels.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VolunteerHub.Backend.Controllers
{
    [Route("api")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IProjectRepository _projectRepository;
        private readonly ITaskRepository _taskRepository;
        
        public TasksController(
            UserManager<User> userManager,
            IProjectRepository projectRepository,
            ITaskRepository taskRepository
            )
        {
            _userManager = userManager;
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
        }

        [HttpGet("tasks/{projectId}")]
        public IActionResult Tasks([FromRoute(Name = "projectId")] string DtoId)
        {
            long Id;
            Project? project;
            if (long.TryParse(DtoId, out Id))
            {
                project = _projectRepository.GetById(Id);
                if(project == null)
                {
                    return BadRequest("Project Not Found");
                }
            }
            else
            {
                return BadRequest("conversion impossible");
            }
            var tasks = _taskRepository.Get(t => t.ProjectId == project.Id);
            return Ok(tasks);
        }

        
        [HttpPost("createTask")]
        public IActionResult CreateTask([FromBody]ProjectTasksDto ProjectTaskDto)
        {
            
            try
            {
                if (ProjectTaskDto.ProjectId == null)
                {
                    return BadRequest("No project found");
                }
                var Task = new ProjectTask
                {
                    ProjectId = (long)ProjectTaskDto.ProjectId,
                    Name = ProjectTaskDto.Name,
                    Action = ProjectTaskDto.Action,
                    Description = ProjectTaskDto.Description,
                    StartDate = DateTime.Now,
                    EndDate = ProjectTaskDto.EndDate,
                    SuccessTreshold = ProjectTaskDto.SuccessTreshold,
                    MeasureUnit = ProjectTaskDto.MeasureUnit,
                    IsTime = ProjectTaskDto.IsTime,
                    NeedsValidation = ProjectTaskDto.NeedsValidation,
                    Status = "InProgress"
                };
                _taskRepository.Add(Task);
                _taskRepository.Save();
                return Ok("Success");
            }
            catch (Exception)
            {
                return BadRequest("Something went wrong");
            }
        }
    }
}
