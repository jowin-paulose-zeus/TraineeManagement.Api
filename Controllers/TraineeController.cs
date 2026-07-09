using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Services;
using TraineeManagement.Api.Interfaces;

namespace TraineeManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TraineeController(ITraineeService traineeService, ILogger<TraineeController> logger) : ControllerBase
    {
        private readonly ITraineeService _traineeService = traineeService;
        private readonly ILogger<TraineeController> _logger = logger;

        [HttpGet]
        public async Task<IActionResult> GettAllTrainees()
        {
            try
            {
                List<TraineeResponseRequest> trainees = await _traineeService.GetAllTrainees();
                if (trainees == null)
                {
                    return NotFound();
                }
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

                if (trainee == null)
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

                if (trainee == null)
                {
                    return BadRequest("Could not create the trainee.");
                }

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
                bool updated = await _traineeService.UpdateTraineeData(id, request);

                if (!updated)
                {
                    return NotFound();
                }
                return Ok();
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
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteTrainee for ID {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while deleting trainee with ID {id}.");
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchTrainees([FromQuery] string searchTerm)
        {
            try
            {
                List<TraineeResponseRequest> trainees = await _traineeService.SearchTrainees(searchTerm);
                return Ok(trainees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SearchTrainees with term '{searchTerm}'", searchTerm);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred during the search operation.");
            }
        }

    }

}