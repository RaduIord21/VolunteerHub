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
    [Route("api/[controller]")]
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

        [Authorize(Roles = "Admin")]
        [HttpGet("AllTasks")]
        public IActionResult AllTasks()
        {
            return Ok(_taskRepository.GetAll());
        }


        [HttpPost("{Id:long}/assignTask")]
        public IActionResult AssignTask(long Id, AssignUsersDto assingeeUserDto){
            var task = _taskRepository.GetById(Id);
            if(task == null){
                return BadRequest("No task found");
            }
            var projectStat = _projectStatsRepository.GetByProjectId(task.ProjectId);
            if(projectStat == null){
                return NotFound();
            }
            var users = assingeeUserDto.UserIds;
            if (users == null){
                return Ok("No users");
            }
            foreach (var u in users){
                UserTask ut = new(){
                    UserId = u,
                    TaskId = Id
                };
                _userTaskRepository.Add(ut);
                _userTaskRepository.Save();
                projectStat.TotalTasksAsigned += 1;
                _projectStatsRepository.Update(projectStat);
                _projectStatsRepository.Save();
            }
            return Ok("Success");
        }

        [HttpGet("{Id:long}/projectMembersForTask")]
        public IActionResult ProjectMembers(long Id){
            var project = _projectRepository.GetById(Id);
            if (project == null){
                return BadRequest("No project found");
            }
            var usersOrgs = _userOrganizationRepository.Get(uo => uo.OrganizationId == project.OrganizationId);
            if (usersOrgs == null){
                return BadRequest("Users Not Found");
            }
            IList<User>? users = new List<User>();
            foreach (var uo in usersOrgs){
                if (uo.UserId == null){
                    continue;
                }
                var u = _userManager.FindByIdAsync(uo.UserId);
                if (u.Result == null){
                    continue;
                }
                users.Add(u.Result);
            }
            return Ok(users);
        }

        [HttpGet("{projectId:long}/tasks")]
        public IActionResult Tasks(long projectId){
            var project = _projectRepository.GetById(projectId);
            if (project == null){
                return BadRequest("No project found");
            }
            var tasks = _taskRepository.Get(t => t.ProjectId == project.Id);
            return Ok(tasks);
        }

        [HttpGet("{Id}/task")]
        public IActionResult Task(long Id){
            var task = _taskRepository.GetById(Id);
            return Ok(task);
        }


        [HttpPost("{Id:long}/updateTask")]
        public IActionResult UpdateTask(long Id, UpdateTaskDto updateTaskDto){
            var projectTask = _taskRepository.GetById(Id);
            if (projectTask == null){
                return BadRequest("Unable to find the task");
            }
            var projectStat = _projectStatsRepository.GetByProjectId(projectTask.ProjectId);
            if (User.Identity == null)      {
                return BadRequest("Not logged in");
            }
            var user = _userManager.GetUserAsync(User);
            if (user.Result == null){
                return BadRequest("No user found");
            }
            List<UserStat> userStatuses = new();
            var projectUsers = _userTaskRepository.Get(ut => ut.TaskId == projectTask.Id);
            foreach (var pu in projectUsers){
                var userStat = _userStatsRepository.GetByUserId(pu.UserId);
                if (userStat == null){
                    continue;
                }
                userStatuses.Add(userStat);
            }
            if (projectStat == null){
                return BadRequest("No Stats found for the project");
            }
            string? Status = "InProgress";
            decimal Progress = updateTaskDto.Progress;
            if (projectTask.EndDate != null) {
                if (DateTime.Compare(updateTaskDto.SubmissionDate, (DateTime)projectTask.EndDate) > 0){
                    Status = "Overdue";
                }else{
                    if (updateTaskDto.Progress > projectTask.SuccessTreshold){
                        Status = "Completed";
                        projectStat.TotalTasksCompleted += 1;
                        foreach (var s in userStatuses){
                            s.TasksCompleted += 1;
                            _userStatsRepository.Update(s);
                        }
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
            _userStatsRepository.Save();
            return Ok(projectTask.ProjectId);
        }

       

        [HttpPost("{Id:long}/editTask")]
        public IActionResult EditTask(long Id, [FromBody] EditTasksDto editTaskDto)
        {
            try
            {
                var projectTask = _taskRepository.GetById(Id);
                if (projectTask == null)
                {
                    return BadRequest("No task found");
                }
                projectTask.Name = editTaskDto.Name;
                projectTask.Description = editTaskDto.Description;
                projectTask.Action = editTaskDto.Action;
                projectTask.IsTime = editTaskDto.IsTime;
                projectTask.MeasureUnit = editTaskDto.MeasureUnit;
                if (projectTask.SuccessTreshold < editTaskDto.SuccessTreshold)
                {
                    projectTask.SuccessTreshold = editTaskDto.SuccessTreshold;
                    projectTask.Status = "InProgress";
                }
                if (projectTask.SuccessTreshold > editTaskDto.SuccessTreshold)
                {
                    projectTask.SuccessTreshold = editTaskDto.SuccessTreshold;
                    projectTask.Status = "Completed";
                }
                if (projectTask.EndDate > editTaskDto.EndDate)
                {
                    projectTask.EndDate = editTaskDto.EndDate;
                    projectTask.Status = "Overdue";
                }

                if (projectTask.EndDate < editTaskDto.EndDate && projectTask.SuccessTreshold < editTaskDto.SuccessTreshold)
                {
                    projectTask.EndDate = editTaskDto.EndDate;
                    projectTask.SuccessTreshold = editTaskDto.SuccessTreshold;
                    projectTask.Status = "InProgress";
                }
                _taskRepository.Update(projectTask);
                _taskRepository.Save();
                return Ok(projectTask.ProjectId);
            }
            catch (Exception)
            {
                return BadRequest("Something went wrong");
            }
        }


        [HttpPost("{Id:long}/DeleteTask")]
        public IActionResult DeleteTask(long Id)
        {
            var task = _taskRepository.GetById(Id);
            if (task == null)
            {
                return BadRequest("Task not found");
            }
            _taskRepository.Delete(task);
            _taskRepository.Save();
            return Ok("Task successfully deleted");
        }

        [HttpGet("{Id:long}/TaskMembers")]
        public IActionResult TaskMembers(long Id) { 
            List<User> users = new List<User>();
            
            var task = _taskRepository.GetById(Id);
                
            var userTasks = _userTaskRepository.Get(ut => ut.TaskId == task.Id);
            if (userTasks == null)
            {
                return BadRequest("No users found");
            }
            foreach (var userTask in userTasks)
            {
                if(userTask.UserId == null)
                {
                    continue;
                }
                var u = _userManager.FindByIdAsync(userTask.UserId);
                if (u.Result == null)
                {
                    continue;   
                }
                users.Add(u.Result);
            }
            return Ok(users);
        }

        [HttpPost("{Id:long}/kickFromTask")]
        public IActionResult KickFromTask(long Id,KickFromTaskDto kickFromTaskDto){
            if (kickFromTaskDto.UserId == null){
                return BadRequest("User not found");
            }           
            var userTask = _userTaskRepository.Get(ut => ut.TaskId == Id && ut.UserId == kickFromTaskDto.UserId);   
            if (userTask == null) {
                return BadRequest("User not in task");
            }
            _userTaskRepository.Delete(userTask[0]);    
            _userTaskRepository.Save();
            return Ok("Success");
        }


        [HttpPost("{projectId:long}/createTask")]
        public IActionResult CreateTask(long projectId,[FromBody]ProjectTasksDto ProjectTaskDto)
        {
            try
            {
                if (ProjectTaskDto.ProjectId == null)
                {
                    return BadRequest("No project found");
                }                
                var Task = new ProjectTask
                {
                    ProjectId = projectId,
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
