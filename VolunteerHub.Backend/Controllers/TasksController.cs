using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VolunteerHub.Backend.Models;
using VolunteerHub.DataAccessLayer.Interfaces;
using VolunteerHub.DataAccessLayer.Repositories;
using VolunteerHub.DataModels.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VolunteerHub.Backend.Controllers
{
    [ApiController]
    [Route("api")]    
    public class TasksController : Controller
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;


        public TasksController(
            ITaskRepository taskRepository,
            IMapper mapper
            )
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        // GET: api/<TasksController>
       
        // POST api/<TasksController>
        [HttpPost("createTask")]
        public IActionResult CreateTask([FromBody] ProjectTasksDto projectTasksDto)
        {
            _taskRepository.Add(_mapper.Map<ProjectTask>(projectTasksDto));
            _taskRepository.Save();
            return Ok("Success");
        }
    }
}
