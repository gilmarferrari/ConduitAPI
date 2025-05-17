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
    public class ChatGroupsController : BaseController
    {
        private readonly IMediator _mediator;

        public ChatGroupsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(RoleList.SystemAdmin, RoleList.SystemUser)]
        [HttpGet]
        public async Task<IEnumerable<ChatGroupResponse>> GetChatGroups([FromQuery] GetChatGroupsRequest query)
        {
            query.LoggedUserID = LoggedUser.ID;

            return await _mediator.Send(query);
        }

        [Authorize(RoleList.SystemAdmin, RoleList.SystemUser)]
        [HttpGet("id")]
        public async Task<ChatGroupResponse> GetChatGroupByID([FromQuery] GetChatGroupByIDRequest query)
        {
            query.LoggedUserID = LoggedUser.ID;

            return await _mediator.Send(query);
        }

        [Authorize(RoleList.SystemAdmin, RoleList.SystemUser)]
        [HttpPost]
        public async Task<OkResponse> CreateChatGroup(CreateChatGroupRequest command)
        {
            command.LoggedUserID = LoggedUser.ID;

            return await _mediator.Send(command);
        }

        [Authorize(RoleList.SystemAdmin, RoleList.SystemUser)]
        [HttpPatch]
        public async Task<OkResponse> UpdateChatGroup(UpdateChatGroupRequest command)
        {
            command.LoggedUserID = LoggedUser.ID;

            return await _mediator.Send(command);
        }

        [Authorize(RoleList.SystemAdmin, RoleList.SystemUser)]
        [HttpPost("participants/add")]
        public async Task<OkResponse> AddChatGroupParticipant(AddChatGroupParticipantRequest command)
        {
            command.LoggedUserID = LoggedUser.ID;

            return await _mediator.Send(command);
        }

        [Authorize(RoleList.SystemAdmin, RoleList.SystemUser)]
        [HttpPost("participants/remove")]
        public async Task<OkResponse> RemoveChatGroupParticipant(RemoveChatGroupParticipantRequest command)
        {
            command.LoggedUserID = LoggedUser.ID;

            return await _mediator.Send(command);
        }

        [Authorize(RoleList.SystemAdmin, RoleList.SystemUser)]
        [HttpPost("quit")]
        public async Task<OkResponse> QuitChatGroup(QuitChatGroupRequest command)
        {
            command.LoggedUserID = LoggedUser.ID;

            return await _mediator.Send(command);
        }

        [Authorize(RoleList.SystemAdmin, RoleList.SystemUser)]
        [HttpGet("messages")]
        public async Task<IEnumerable<ChatGroupMessageResponse>> GetChatGroupMessages([FromQuery] GetChatGroupMessagesRequest query)
        {
            query.LoggedUserID = LoggedUser.ID;

            return await _mediator.Send(query);
        }

        [Authorize(RoleList.SystemAdmin, RoleList.SystemUser)]
        [HttpPost("messages")]
        public async Task<OkResponse> SendChatGroupMessage(SendChatGroupMessageRequest command)
        {
            command.SenderID = LoggedUser.ID;

            return await _mediator.Send(command);
        }

        [Authorize(RoleList.SystemAdmin, RoleList.SystemUser)]
        [HttpDelete("messages")]
        public async Task<OkResponse> DeleteChatGroupMessage([FromQuery] DeleteChatGroupMessageRequest command)
        {
            command.LoggedUserID = LoggedUser.ID;

            return await _mediator.Send(command);
        }
    }
}
