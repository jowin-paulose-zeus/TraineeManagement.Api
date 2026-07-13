using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Services;
using TraineeManagement.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class MentorController(IMentorService mentorService,ILogger<AuthService> logger) : ControllerBase
    {
        private readonly IMentorService _mentorService = mentorService;
        private readonly ILogger<AuthService> _logger = logger;

        [HttpGet]
        public async Task<IActionResult> GetMentors([FromQuery] MentorQuery query)
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
                PagedResponse<MentorResponse> mentors = await _mentorService.GetMentors(query);
                if (mentors == null)
                {
                    _logger.LogInformation("Record not found in Mentors");
                    return NotFound();
                }
                _logger.LogInformation("Record found in Mentors");
                return Ok(mentors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GettAllMentors");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving mentors.");
            }
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetMentorById(int id)
        {
            try
            {
                MentorResponse? mentor = await _mentorService.GetMentorById(id);

                if (mentor == null)
                {
                    return NotFound();
                }
                return Ok(mentor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetTraineeById for ID {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while retrieving trainee with ID {id}.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddMentor([FromBody] MentorRequest request)
        {
            try
            {
                MentorResponse? mentor = await _mentorService.AddMentor(request);

                if (mentor == null)
                {
                    return BadRequest("Could not create the mentor.");
                }

                _logger.LogInformation("Mentor created: ID {Id} Email:{Email}",mentor.Id,mentor.Email);
                return CreatedAtAction(
                    nameof(GetMentorById),
                    new { id = mentor.Id },
                    mentor
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddMentor");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the mentor.");
            }
        }

        [HttpPut("id")]
        public async Task<IActionResult> UpdateMentorData(int id, [FromBody] MentorRequest request)
        {
            try
            {
                MentorResponse? updatedmentor = await _mentorService.UpdateMentorData(id, request);

                if (updatedmentor == null)
                {
                    _logger.LogWarning("Record not found in Mentor");
                    return NotFound();
                }
                _logger.LogInformation("Mentor updated. ID: {Id}",id);
                return Ok(updatedmentor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateMentorData for ID {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating mentor with ID {id}.");
            }
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeleteMentor(int id)
        {
            try
            {
                bool deleted = await _mentorService.DeleteMentor(id);

                if (!deleted)
                {
                    _logger.LogWarning("Record not found in Mentors");
                    return NotFound();
                }
                _logger.LogInformation("Mentor deleted. ID: {Id}",id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteMentor for ID {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while deleting mentor with ID {id}.");
            }
        }
    }

}