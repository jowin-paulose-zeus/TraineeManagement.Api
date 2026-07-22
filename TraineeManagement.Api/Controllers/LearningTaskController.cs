using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Services;
using TraineeManagement.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using TraineeManagement.Data.Models;

namespace TraineeManagement.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class LearningTaskController(ILearningTaskService learningtaskService,ILogger<AuthService> logger) : ControllerBase
    {
        private readonly ILearningTaskService _learningtaskService = learningtaskService;
        private readonly ILogger<AuthService> _logger = logger;

        [HttpGet]
        public async Task<IActionResult> GetLearningTasks([FromQuery] LearningTaskQuery query)
        {
            if (query.PageNumber < 1)
            {
                return BadRequest("Page Number must be greater than 0");
            }
            if (query.PageSize < 1)
            {
                return BadRequest("Page size must be greater than 0");
            }
            try
            {
                PagedResponse<LearningTaskResponse> learningtasks = await _learningtaskService.GetLearningTasks(query);
                if (learningtasks is null)
                {
                    _logger.LogInformation("Record not found in LearningTasks");
                    return NotFound();
                }
                _logger.LogInformation("Record found in LearningTasks");
                return Ok(learningtasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GettAllLearningTasks");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving learningtasks.");
            }
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetLearningTaskById(int id)
        {
            try
            {
                LearningTaskResponse? learningtask = await _learningtaskService.GetLearningTaskById(id);

                if (learningtask is null)
                {
                    return NotFound();
                }
                return Ok(learningtask);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetTraineeById for ID {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while retrieving trainee with ID {id}.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddLearningTask([FromBody] LearningTaskRequest request)
        {
            try
            {
                LearningTaskResponse? learningtask = await _learningtaskService.AddLearningTask(request);

                if (learningtask is null)
                {
                    return BadRequest("Could not create the learningtask.");
                }

                _logger.LogInformation("LearningTask created: ID {Id} Title:{Title}",learningtask.Id,learningtask.Title);
                return CreatedAtAction(
                    nameof(GetLearningTaskById),
                    new { id = learningtask.Id },
                    learningtask
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddLearningTask");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the learningtask.");
            }
        }

        [HttpPut("id")]
        public async Task<IActionResult> UpdateLearningTaskData(int id, [FromBody] LearningTaskRequest request)
        {
            try
            {
                LearningTaskResponse? updatedlearningtask = await _learningtaskService.UpdateLearningTaskData(id, request);

                if (updatedlearningtask is null)
                {
                    _logger.LogWarning("Record not found in LearningTask");
                    return NotFound();
                }
                _logger.LogInformation("LearningTask updated. ID: {Id}",id);
                return Ok(updatedlearningtask);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateLearningTaskData for ID {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating learningtask with ID {id}.");
            }
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeleteLearningTask(int id)
        {
            try
            {
                bool deleted = await _learningtaskService.DeleteLearningTask(id);

                if (!deleted)
                {
                    _logger.LogWarning("Record not found in LearningTasks");
                    return NotFound();
                }
                _logger.LogInformation("LearningTask deleted. ID: {Id}",id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteLearningTask for ID {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while deleting learningtask with ID {id}.");
            }
        }
    }

}