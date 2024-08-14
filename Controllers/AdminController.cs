using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityAuthWithScratch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet("Admins")]
        public IActionResult Admin()
        {
            return Ok(new {message = "You have accessed the Admin controller." });
        }

        [Authorize(Roles = "User")]
        [HttpGet("Users")]
        public IActionResult User()
        {
            return Ok(new { message = "You have accessed the User controller." });
        }

        [Authorize(Roles = "Assistant")]
        [HttpGet("Assistants")]
        public IActionResult Assistant()
        {
            return Ok(new { message = "You have accessed the Assistant controller." });
        }

    }
}
