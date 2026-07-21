using Microsoft.AspNetCore.Mvc;

namespace TraineeManagement.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetHealth()
        {
            return Ok(new
            {
                status = "Running",
                application = "TraineeManagement.api",
                timestamp = DateTime.UtcNow
            });
        }
    }
}