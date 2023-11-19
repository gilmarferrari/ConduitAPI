using MediatR;
using Microsoft.AspNetCore.Mvc;
using Model.Commands.Requests.Post;
using Model.Commands.Requests.Delete;
using Model.Commands.Requests.Put;
using Model.Commands.Responses;
using Model.Queries.Requests;
using Model.Queries.Responses;

namespace ConduitAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsersController : BaseController
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize("SystemAdmin", "SystemUser")]
        [HttpGet]
        public async Task<IEnumerable<UserResponse>> GetUsers([FromQuery] GetUsersRequest query)
        {
            return await _mediator.Send(query);
        }

        [Authorize("SystemAdmin", "SystemUser")]
        [HttpGet("id")]
        public async Task<ActionResult<UserResponse>> GetUserByID([FromQuery] GetUserByIDRequest query)
        {
            if (LoggedUser.ID != query.ID && !LoggedUser.IsAdmin)
            {
                return Forbid();
            }

            return await _mediator.Send(query);
        }

        [Authorize("SystemAdmin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<OkResponse>> UpdateUser(int id, UpdateUserRequest command)
        {
            if (id != command.ID)
            {
                return BadRequest();
            }

            return await _mediator.Send(command);
        }

        [HttpPost]
        public async Task<ActionResult<OkResponse>> CreateUserAccount(CreateUserAccountRequest command)
        {
            return await _mediator.Send(command);
        }

        [Authorize("SystemAdmin", "SystemUser")]
        [HttpDelete]
        public async Task<ActionResult<OkResponse>> DeleteUserAccount([FromQuery] DeleteUserAccountRequest command)
        {
            if (!LoggedUser.IsAdmin && command.ID != LoggedUser.UserAccountID)
            {
                return Forbid();
            }

            return await _mediator.Send(command);
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<AuthenticationResponse>> Authenticate(AuthenticateRequest command)
        {
            command.IpAddress = IPAddress;
            command.Agent = Agent;
            command.Origin = Origin;

            return await _mediator.Send(command);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthenticationResponse>> RefreshToken(RefreshTokenRequest command)
        {
            command.IpAddress = IPAddress;
            command.Agent = Agent;
            command.Origin = Origin;

            return await _mediator.Send(command);
        }

        [Authorize("SystemAdmin", "SystemUser")]
        [HttpPost("revoke-token")]
        public async Task<ActionResult<OkResponse>> RevokeToken(RevokeTokenRequest command)
        {
            command.IpAddress = IPAddress;
            command.Agent = Agent;
            command.Origin = Origin;

            if (!LoggedUser.OwnsToken(command.RefreshToken))
            {
                return Forbid();
            }

            return await _mediator.Send(command);
        }

        [HttpPost("activate-account")]
        public async Task<ActionResult<OkResponse>> ActivateAccount(ActivateAccountRequest command)
        {
            command.IpAddress = IPAddress;
            command.Agent = Agent;
            command.Origin = Origin;

            return await _mediator.Send(command);
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<OkResponse>> ResetPassword(ResetPasswordRequest command)
        {
            command.IpAddress = IPAddress;
            command.Agent = Agent;
            command.Origin = Origin;

            return await _mediator.Send(command);
        }

        [HttpPost("resend-code")]
        public async Task<ActionResult<OkResponse>> ResendCode(ResendCodeRequest command)
        {
            return await _mediator.Send(command);
        }
    }
}
