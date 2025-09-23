using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Commands;
using Application.Queries;
using Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UsersController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(Create), new { id = result.Id }, result);
            }
            catch (System.InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserDto>>> Get()
        {
            var result = await _mediator.Send(new GetUsersQuery());
            return Ok(result);
        }
    }
}
