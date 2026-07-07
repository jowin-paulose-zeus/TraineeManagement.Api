using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Services;

namespace TraineeManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TraineeController(ITraineeService traineeService) : ControllerBase
    {
        private readonly ITraineeService _traineeService = traineeService;

        [HttpGet]
        public async Task<IActionResult> GettAllTrainees()
        {
            var trainees = await _traineeService.GetAllTrainees();
            return Ok(trainees);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetTraineeById(int id)
        {
            TraineeResponseRequest? trainee = await _traineeService.GetTraineeById(id);

            if (trainee == null)
            {
                return NotFound();
            }
            return Ok(trainee);
        }

        [HttpPost]
        public async Task<IActionResult> AddTrainee([FromBody] CreateTraineeRequest request)
        {
            TraineeResponseRequest? trainee = await _traineeService.AddTrainee(request);

            return CreatedAtAction(
                nameof(GetTraineeById),
                new { id = trainee.Id },
                trainee
            );
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTraineeData(int id, UpdateTraineeRequest request)
        {
            bool updated = await _traineeService.UpdateTraineeData(id, request);

            if (!updated)
            {
                return NotFound();
            }
            return Ok();
        }
        [HttpDelete("id")]
        public async Task<IActionResult> DeleteTrainee(int id)
        {
            bool updated = await _traineeService.DeleteTrainee(id);

            if (!updated)
            {
                return NotFound();
            }
            return NoContent();
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchTrainees(string searchTerm)
        {
            List<TraineeResponseRequest> trainees = await _traineeService.SearchTrainees(searchTerm);
            return Ok(trainees);
        }
    }

}