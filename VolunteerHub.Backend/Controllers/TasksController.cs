using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VolunteerHub.Backend.Models;
using VolunteerHub.DataModels.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VolunteerHub.Backend.Controllers
{
    [Route("api")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        public TasksController(
            UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [Authorize(Roles = "Coordinator")]
        [Authorize(Roles ="Admin")]
        [HttpPost("createTask")]
        public IActionResult createTask(ProjectTasksDto ProjectTaskDto)
        {
            
            try
            {
                var userID = _userManager.FindByNameAsync(ProjectTaskDto.AssigneeName);
                if (userID.Result == null)
                {
                    return BadRequest("No user found");
                }

                var Task = new ProjectTask
                {
                    AssigneeId = userID.Result.Id,
                    Name = ProjectTaskDto.Name,
                    Description = ProjectTaskDto.Description,
                    StartDate = DateTime.Now,
                    EndDate = ProjectTaskDto.EndDate,
                    SuccessTreshold = ProjectTaskDto.SuccessTreshold,
                    MeasureUnit = ProjectTaskDto.MeasureUnit,
                    IsTime = ProjectTaskDto.IsTime,
                    NeedsValidation = ProjectTaskDto.NeedsValidation,
                    Status = "InProgress"
                };
                return Ok(Task);
            }
            catch (Exception)
            {
                return BadRequest("Something went wrong");
            }
        }
    }
}
