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
    public class MessagesController : BaseController
    {
        private readonly IMediator _mediator;

        public MessagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(RoleList.SystemAdmin, RoleList.SystemUser)]
        [HttpGet]
        public async Task<IEnumerable<MessageResponse>> GetMessages([FromQuery] GetMessagesRequest query)
        {
            query.LoggedUserID = LoggedUser.ID;

            return await _mediator.Send(query);
        }

        [Authorize(RoleList.SystemAdmin, RoleList.SystemUser)]
        [HttpPost]
        public async Task<OkResponse> SendMessage(SendMessageRequest command)
        {
            command.SenderID = LoggedUser.ID;

            return await _mediator.Send(command);
        }

        [Authorize(RoleList.SystemAdmin, RoleList.SystemUser)]
        [HttpPost("read")]
        public async Task<OkResponse> ReadMessages(ReadMessagesRequest command)
        {
            command.LoggedUserID = LoggedUser.ID;

            return await _mediator.Send(command);
        }

        [Authorize(RoleList.SystemAdmin, RoleList.SystemUser)]
        [HttpDelete]
        public async Task<OkResponse> DeleteMessage([FromQuery] DeleteMessageRequest command)
        {
            command.LoggedUserID = LoggedUser.ID;

            return await _mediator.Send(command);
        }
    }
}
