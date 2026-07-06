using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Services;

namespace TraineeManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TraineeController : ControllerBase
    {
        private readonly ITraineeService _traineeService;

        public TraineeController(ITraineeService traineeService)
        {
            _traineeService = traineeService;
        }
        [HttpGet]
        public IActionResult GettAllTrainees()
        {
            return Ok(_traineeService.GetAllTrainees());
        }
       
        [HttpGet("id")]
        public IActionResult GetTraineeById(int id)
        {
            var trainee = _traineeService.GetTraineeById(id);

            if (trainee == null)
            {
                return NotFound();
            }
            return Ok(trainee);
        }
    
        [HttpPost]
        public IActionResult AddTrainee([FromBody] CreateTraineeRequest request)
        {
            var trainee = _traineeService.AddTrainee(request);

            return CreatedAtAction(
                nameof(GetTraineeById),
                new { id = trainee.Id},
                trainee
            );
        }

        [HttpPut]
        public IActionResult UpdateTraineeData(int id, UpdateTraineeRequest request)
        {
            var updated = _traineeService.UpdateTraineeData(id, request);

            if (!updated)
            {
                return NotFound();
            }
            return Ok();
        }
        [HttpDelete("id")]
        public IActionResult DeleteTrainee(int id)
        {
            var updated = _traineeService.DeleteTrainee(id);

            if (!updated)
            {
                return NotFound();
            }
            return NoContent();
        }
    }

}