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
    public class MessagesController : BaseController
    {
        private readonly IMediator _mediator;

        public MessagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize("SystemAdmin", "SystemUser")]
        [HttpGet]
        public async Task<IEnumerable<MessageResponse>> GetMessages([FromQuery] GetMessagesRequest query)
        {
            query.UserID = LoggedUser.ID;

            return await _mediator.Send(query);
        }

        [Authorize("SystemAdmin", "SystemUser")]
        [HttpPost]
        public async Task<OkResponse> SendMessage(SendMessageRequest command)
        {
            command.SenderID = LoggedUser.ID;

            return await _mediator.Send(command);
        }

        [Authorize("SystemAdmin", "SystemUser")]
        [HttpPut("read")]
        public async Task<OkResponse> ReadMessages(ReadMessagesRequest command)
        {
            command.UserID = LoggedUser.ID;

            return await _mediator.Send(command);
        }

        [Authorize("SystemAdmin", "SystemUser")]
        [HttpDelete]
        public async Task<OkResponse> DeleteMessage([FromQuery] DeleteMessageRequest command)
        {
            command.UserID = LoggedUser.ID;

            return await _mediator.Send(command);
        }
    }
}
