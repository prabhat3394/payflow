using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayFlow.Identity.Api.Application.Commands;
using PayFlow.Identity.Api.Application.Queries;

namespace PayFlow.Identity.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly MediatR.IMediator _mediator;
        
        public AuthController(MediatR.IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok(new { UserId = result.UserId , message = "User registered successfully."});
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok(new { AccessToken = result.AccessToken, RefreshToken = result.RefreshToken });
        }

        [HttpGet("loggedInUser")]
        [Authorize]
        public async Task<IActionResult> LoggedInUser()
        {
            var result = await _mediator.Send(new GetCurrentUserQuery(User));
            if(!result.Found)
            {
                return NotFound(new { error = result.Error });
            }

            return Ok(new
            {
                userId = result.UserId,
                fullName = result.FullName,
                email = result.Email,
                createdAt = result.CreatedAt
            });
        }
    }
}
