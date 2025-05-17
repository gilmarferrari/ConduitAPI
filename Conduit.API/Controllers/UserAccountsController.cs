using Conduit.Application.Commands.Requests;
using Conduit.Application.Queries.Requests;
using Conduit.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Conduit.Application.Commands.Responses;
using Conduit.Application.Queries.Responses;
using Conduit.API.Attributes;

namespace Conduit.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserAccountsController : BaseController
    {
        private readonly IMediator _mediator;

        public UserAccountsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<UserAccountResponse> GetUserAccount([FromQuery] GetUserAccountRequest query)
        {
            return await _mediator.Send(query);
        }

        [HttpPost]
        public async Task<ActionResult<OkResponse>> CreateUserAccount(CreateUserAccountRequest command)
        {
            return await _mediator.Send(command);
        }

        [Authorize(RoleList.SystemAdmin, RoleList.SystemUser)]
        [HttpDelete]
        public async Task<ActionResult<OkResponse>> DeleteUserAccount([FromQuery] DeleteUserAccountRequest command)
        {
            if (!LoggedUser.IsAdmin && command.Email != LoggedUser.UserAccount.Email)
            {
                return Forbid();
            }

            return await _mediator.Send(command);
        }

        [HttpPost("activate")]
        public async Task<ActionResult<OkResponse>> ActivateAccount(ActivateAccountRequest command)
        {
            command.IpAddress = IPAddress;
            command.Agent = Agent;
            command.Origin = Origin;

            return await _mediator.Send(command);
        }
    }
}
