using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Commands;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator) => _mediator = mediator;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthUserCommand command)
        {
            try
            {
                var token = await _mediator.Send(command);
                return Ok(new { token });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }
        }
    }
}
