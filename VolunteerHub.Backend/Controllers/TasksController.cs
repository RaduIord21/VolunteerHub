using AutoMapper;
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
        private readonly IProjectStatsRepository _projectStatsRepository;
        private readonly IUserStatsRepository _userStatsRepository;
        private readonly IUserTaskRepository _userTaskRepository;
        private readonly IUserOrganizationRepository _userOrganizationRepository;
        public TasksController(
            UserManager<User> userManager,
            IProjectRepository projectRepository,
            ITaskRepository taskRepository,
            IProjectStatsRepository projectStatsRepository,
            IUserStatsRepository userStatsRepository,
            IUserTaskRepository userTaskRepository,
            IUserOrganizationRepository userOrganizationRepository)
        {
            _userManager = userManager;
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
            _projectStatsRepository = projectStatsRepository;
            _userStatsRepository = userStatsRepository;
            _userTaskRepository = userTaskRepository;
            _userOrganizationRepository = userOrganizationRepository;
        }

        [HttpPost("assignTask")]
        public IActionResult AssignTask(AssignUsersDto assingeeUserDto)
        {
            
            var users = assingeeUserDto.Users;
            if (users == null)
            {
                return Ok("No users");
            }
            foreach (var u in users)
            {
                UserTask ut = new();
                ut.UserId = u.Id;
                ut.TaskId = assingeeUserDto.TaskId;
                _userTaskRepository.Add(ut);
                _userTaskRepository.Save();
            }
            return Ok("Success");
        }

        [HttpGet("projectMembersForTask/{projectId}")]
        public IActionResult ProjectMembers([FromRoute(Name = "projectId")] string projectId)
        {
            long Id;
            Project? project;
            if (long.TryParse(projectId, out Id))
            {
                project = _projectRepository.GetById(Id);
                if (project == null)
                {
                    return BadRequest("Project Not Found");
                }
            }
            else
            {
                return BadRequest("conversion impossible");
            }
            //var users = _userManager.Users.Where(u => u.OrganizationId == project.OrganizationId);

            var usersOrgs = _userOrganizationRepository.Get(uo => uo.OrganizationId == project.OrganizationId);
            if (usersOrgs == null)
            {
                return BadRequest("Users Not Found");
            }
            IList<User>? users = new List<User>();
            foreach (var uo in usersOrgs)
            {
                if(uo.UserId == null)
                {
                    continue;
                }
                var u  = _userManager.FindByIdAsync(uo.UserId);
                if (u.Result == null)
                {
                    continue;
                }
                users.Add(u.Result);
            }
            return Ok(users);
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

        [HttpPost("updateTask")]

        public IActionResult UpdateTask(UpdateTaskDto updateTaskDto)
        {
            var projectTask = _taskRepository.GetById(updateTaskDto.Id);
            if (projectTask == null)
            {
                return BadRequest("Unable to find the task");
            }
            var projectStat = _projectStatsRepository.GetByProjectId(projectTask.ProjectId);
            var userStat = _userStatsRepository.GetByUserId(projectTask.AssigneeId);
            if (userStat == null)
            {
                return BadRequest("No Stats found for the user");
            }
            if (projectStat == null)
            {
                return BadRequest("No Stats found for the project");
            }
            string? Status = "InProgress";
            decimal Progress = updateTaskDto.Progress;
            if(projectTask.EndDate != null) {
                if (DateTime.Compare(updateTaskDto.SubmissionDate, (DateTime)projectTask.EndDate) > 0)
                {
                    Status = "Overdue";
                }
                else
                {
                    if (updateTaskDto.Progress > projectTask.SuccessTreshold)
                    {
                        Status = "Completed";
                        projectStat.TotalTasksCompleted += 1;
                        userStat.TasksCompleted += 1;
                    }  
                }
            }
            projectTask.Progress += Progress;
            projectTask.Status = Status;
            _taskRepository.Update(projectTask);
            _taskRepository.Save();

            projectStat.UpdatedAt = DateTime.Now;

            _projectStatsRepository.Update(projectStat);
            _projectStatsRepository.Save();
            
            _userStatsRepository.Update(userStat);
            _userStatsRepository.Save();
            return Ok("Success");
        }

        [HttpGet("task/{taskId}")]
        public IActionResult Task([FromRoute(Name = "taskId")] string taskId)
        {
            long Id;
            ProjectTask? task;
            if (long.TryParse(taskId, out Id))
            {
               task = _taskRepository.GetById(Id);
                if (task == null)
                {
                    return BadRequest("Project Not Found");
                }
            }
            else
            {
                return BadRequest("conversion impossible");
            }

            return Ok(task);
        }

        [HttpPost("editTask")]
        public IActionResult EditTask([FromBody] EditTasksDto editTaskDto)
        {
            try
            {   
                var projectTask = _taskRepository.GetById(editTaskDto.Id);
                if(projectTask == null)
                {
                    return BadRequest("No task found");
                }
                projectTask.Name = editTaskDto.Name;
                projectTask.Description = editTaskDto.Description;
                projectTask.Action = editTaskDto.Action;
                projectTask.EndDate = editTaskDto.EndDate;
                projectTask.SuccessTreshold = editTaskDto.SuccessTreshold;
                projectTask.IsTime = editTaskDto.IsTime;
                _taskRepository.Update(projectTask);
                _taskRepository.Save();
                return Ok("Success");
            }
            catch (Exception)
            {
                return BadRequest("Something went wrong");
            }
        }


        [HttpPost("DeleteTask")]
        public IActionResult DeleteTask([FromBody]DeleteTaskDto deleteTaskDto)
        {
            var task = _taskRepository.GetById(deleteTaskDto.Id);
            if(task == null)
            {
                return BadRequest("Task not found");
            }
            _taskRepository.Delete(task);
            _taskRepository.Save();
            return Ok("Task successfully deleted");
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
                    Status = "InProgress"
                };

                var projectStat = _projectStatsRepository.GetByProjectId(Task.ProjectId);
                if (projectStat == null)
                {
                    return BadRequest("Project stat not found");
                }
                projectStat.TotalTasksUncompleted += 1;
                _projectStatsRepository.Update(projectStat);
                _projectStatsRepository.Save();
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
