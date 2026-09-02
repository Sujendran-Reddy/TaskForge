using Microsoft.AspNetCore.Mvc;

namespace TaskForge.Api.Controllers
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
                status = "TaskForge API is running"
            });
        }
    }
}