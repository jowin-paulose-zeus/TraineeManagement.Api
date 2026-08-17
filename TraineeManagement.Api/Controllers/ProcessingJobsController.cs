using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Interfaces;
using TraineeManagement.Data.Enums;

namespace TraineeManagement.Api.Controllers;

[ApiController]
[Route("api/processing-jobs")]
[Authorize(Roles = nameof(UserRoles.Admin))]
public class ProcessingJobsController(IProcessingJobService service, ILogger<ProcessingJobsController> logger) : ControllerBase
{
    private readonly IProcessingJobService _service = service;
    private readonly ILogger<ProcessingJobsController> _logger = logger;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            ProcessingJobResponse job = await _service.GetByIdAsync(id);

            return Ok(job);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}