using MediatR;
using Microsoft.AspNetCore.Mvc;
using Conduit.Application.Commands.Responses;
using Conduit.Application.Queries.Requests;
using Conduit.Application.Queries.Responses;
using Conduit.Application.Commands.Requests;
using Conduit.Domain.Entities;
using Conduit.API.Attributes;

namespace Conduit.API.Controllers
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

        [Authorize(RoleList.SystemAdmin, RoleList.SystemUser)]
        [HttpGet]
        public async Task<IEnumerable<UserResponse>> GetUsers([FromQuery] GetUsersRequest query)
        {
            return await _mediator.Send(query);
        }

        [Authorize(RoleList.SystemAdmin, RoleList.SystemUser)]
        [HttpGet("id")]
        public async Task<ActionResult<UserResponse>> GetUserByID([FromQuery] GetUserByIDRequest query)
        {
            if (LoggedUser.ID != query.ID && !LoggedUser.IsAdmin)
            {
                return Forbid();
            }

            return await _mediator.Send(query);
        }

        [Authorize(RoleList.SystemAdmin)]
        [HttpPatch("{id}")]
        public async Task<ActionResult<OkResponse>> UpdateUser(Guid id, UpdateUserRequest command)
        {
            if (id != command.ID)
            {
                return BadRequest();
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

        [Authorize(RoleList.SystemAdmin, RoleList.SystemUser)]
        [HttpPost("revoke-token")]
        public async Task<ActionResult<OkResponse>> RevokeToken(RevokeTokenRequest command)
        {
            command.IpAddress = IPAddress;
            command.Agent = Agent;
            command.Origin = Origin;

            if (!LoggedUser.OwnsToken(command.LongLivedToken))
            {
                return Forbid();
            }

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
