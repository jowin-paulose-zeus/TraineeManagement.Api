using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Services;
using TraineeManagement.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace TraineeManagement.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class TaskAssignmentController(ITaskAssignmentService taskAssignmentService, ILogger<AuthService> logger) : ControllerBase
    {
        private readonly ITaskAssignmentService _taskAssignmentService = taskAssignmentService;
        private readonly ILogger<AuthService> _logger = logger;

        [HttpPost]
        public async Task<IActionResult> Create(TaskAssignmentRequest request)
        {
            _logger.LogInformation("Creating task assignment for TraineeId: {TraineeId}", request.TraineeId);

            TaskAssignmentResponse? assignment = await _taskAssignmentService.AddTaskAssignment(request);

            if (assignment == null)
            {
                _logger.LogWarning("Task assignment creation failed.");

                return BadRequest("Invalid task assignment details");
            }

            _logger.LogInformation("Task assignment created successfully. Id: {Id}", assignment.Id);

            return CreatedAtAction(nameof(GetById), new { id = assignment.Id }, assignment);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Fetching all task assignments.");

            List<TaskAssignmentResponse> assignments = await _taskAssignmentService.GetTaskAssignments();

            return Ok(assignments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Fetching task assignment with Id: {Id}", id);

            TaskAssignmentResponse? assignment = await _taskAssignmentService.GetTaskAssignmentById(id);

            if (assignment == null)
            {
                _logger.LogWarning("Task assignment not found. Id: {Id}", id);

                return NotFound();
            }

            return Ok(assignment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TaskAssignmentRequest request)
        {
            _logger.LogInformation("Updating task assignment with Id: {Id}", id);

            TaskAssignmentResponse? assignment = await _taskAssignmentService.UpdateTaskAssignment(id, request);

            if (assignment == null)
            {
                _logger.LogWarning("Task assignment not found or update failed. Id: {Id}", id);

                return NotFound();
            }

            _logger.LogInformation("Task assignment updated successfully. Id: {Id}", id);

            return Ok(assignment);
        }


    }
}