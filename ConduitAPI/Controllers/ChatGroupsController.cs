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
    public class ChatGroupsController : BaseController
    {
        private readonly IMediator _mediator;

        public ChatGroupsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize("SystemAdmin", "SystemUser")]
        [HttpGet]
        public async Task<IEnumerable<ChatGroupResponse>> GetChatGroups([FromQuery] GetChatGroupsRequest query)
        {
            query.UserID = LoggedUser.ID;

            return await _mediator.Send(query);
        }

        [Authorize("SystemAdmin", "SystemUser")]
        [HttpGet("id")]
        public async Task<ChatGroupResponse> GetChatGroupByID([FromQuery] GetChatGroupByIDRequest query)
        {
            query.UserID = LoggedUser.ID;

            return await _mediator.Send(query);
        }

        [Authorize("SystemAdmin", "SystemUser")]
        [HttpPost]
        public async Task<OkResponse> CreateChatGroup(CreateChatGroupRequest command)
        {
            command.UserID = LoggedUser.ID;

            return await _mediator.Send(command);
        }

        [Authorize("SystemAdmin", "SystemUser")]
        [HttpPut]
        public async Task<OkResponse> UpdateChatGroup(UpdateChatGroupRequest command)
        {
            command.UserID = LoggedUser.ID;

            return await _mediator.Send(command);
        }

        [Authorize("SystemAdmin", "SystemUser")]
        [HttpPost("participants/add")]
        public async Task<OkResponse> AddChatGroupParticipant(AddChatGroupParticipantRequest command)
        {
            command.UserID = LoggedUser.ID;

            return await _mediator.Send(command);
        }

        [Authorize("SystemAdmin", "SystemUser")]
        [HttpPost("participants/remove")]
        public async Task<OkResponse> RemoveChatGroupParticipant(RemoveChatGroupParticipantRequest command)
        {
            command.UserID = LoggedUser.ID;

            return await _mediator.Send(command);
        }

        [Authorize("SystemAdmin", "SystemUser")]
        [HttpPost("quit")]
        public async Task<OkResponse> QuitChatGroup(QuitChatGroupRequest command)
        {
            command.UserID = LoggedUser.ID;

            return await _mediator.Send(command);
        }

        [Authorize("SystemAdmin", "SystemUser")]
        [HttpGet("messages")]
        public async Task<IEnumerable<ChatGroupMessageResponse>> GetChatGroupMessages([FromQuery] GetChatGroupMessagesRequest query)
        {
            query.UserID = LoggedUser.ID;

            return await _mediator.Send(query);
        }

        [Authorize("SystemAdmin", "SystemUser")]
        [HttpPost("messages")]
        public async Task<OkResponse> SendChatGroupMessage(SendChatGroupMessageRequest command)
        {
            command.SenderID = LoggedUser.ID;

            return await _mediator.Send(command);
        }

        [Authorize("SystemAdmin", "SystemUser")]
        [HttpDelete("messages")]
        public async Task<OkResponse> DeleteChatGroupMessage([FromQuery] DeleteChatGroupMessageRequest command)
        {
            command.UserID = LoggedUser.ID;

            return await _mediator.Send(command);
        }
    }
}
