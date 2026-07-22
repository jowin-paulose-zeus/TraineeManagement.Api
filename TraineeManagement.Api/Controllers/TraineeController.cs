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
    public class TraineeController(ITraineeService traineeService,ILogger<AuthService> logger) : ControllerBase
    {
        private readonly ITraineeService _traineeService = traineeService;
        private readonly ILogger<AuthService> _logger = logger;

        [HttpGet]
        public async Task<IActionResult> GetTrainees([FromQuery] TraineeQuery query)
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
                PagedResponse<TraineeResponseRequest> trainees = await _traineeService.GetTrainees(query);
                if (trainees is null)
                {
                    _logger.LogInformation("Record not found in Trainees");
                    return NotFound();
                }
                _logger.LogInformation("Record found in Trainees");
                return Ok(trainees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GettAllTrainees");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving trainees.");
            }
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetTraineeById(int id)
        {
            try
            {
                TraineeResponseRequest? trainee = await _traineeService.GetTraineeById(id);

                if (trainee is null)
                {
                    return NotFound();
                }
                return Ok(trainee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetTraineeById for ID {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while retrieving trainee with ID {id}.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddTrainee([FromBody] TraineeRequest request)
        {
            try
            {
                TraineeResponseRequest? trainee = await _traineeService.AddTrainee(request);

                if (trainee is null)
                {
                    return BadRequest("Could not create the trainee.");
                }

                _logger.LogInformation("Trainee created: ID {Id} Email:{Email}",trainee.Id,trainee.Email);
                return CreatedAtAction(
                    nameof(GetTraineeById),
                    new { id = trainee.Id },
                    trainee
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddTrainee");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the trainee.");
            }
        }

        [HttpPut("id")]
        public async Task<IActionResult> UpdateTraineeData(int id, [FromBody] TraineeRequest request)
        {
            try
            {
                TraineeResponseRequest? updatedtrainee = await _traineeService.UpdateTraineeData(id, request);

                if (updatedtrainee is null)
                {
                    _logger.LogWarning("Record not found in Trainees");
                    return NotFound();
                }
                _logger.LogInformation("Trainee updated. ID: {Id}",id);
                return Ok(updatedtrainee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateTraineeData for ID {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating trainee with ID {id}.");
            }
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeleteTrainee(int id)
        {
            try
            {
                bool deleted = await _traineeService.DeleteTrainee(id);

                if (!deleted)
                {
                    _logger.LogWarning("Record not found in Trainees");
                    return NotFound();
                }
                _logger.LogInformation("Trainee deleted. ID: {Id}",id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteTrainee for ID {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while deleting trainee with ID {id}.");
            }
        }
    }

}