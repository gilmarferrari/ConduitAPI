using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Conduit.Application.Commands.Responses;
using Conduit.Application.Queries.Requests;
using Conduit.Application.Queries.Responses;
using Conduit.Application.Helpers;
using Conduit.Application.SignalR;
using Conduit.Application.Commands.Requests;
using Conduit.Infrastructure.Data;
using Conduit.Domain.Entities;

namespace Conduit.Application.Services
{
    public interface IMessagingService
    {
        Task<IEnumerable<MessageResponse>> GetMessages(GetMessagesRequest request);
        Task<OkResponse> ReadMessages(ReadMessagesRequest request);
        Task<OkResponse> SendMessage(SendMessageRequest request);
        Task<OkResponse> DeleteMessage(DeleteMessageRequest request);
        Task<IEnumerable<ChatGroupResponse>> GetChatGroups(GetChatGroupsRequest request);
        Task<ChatGroupResponse> GetChatGroupByID(GetChatGroupByIDRequest request);
        Task<OkResponse> CreateChatGroup(CreateChatGroupRequest request);
        Task<OkResponse> UpdateChatGroup(UpdateChatGroupRequest request);
        Task<OkResponse> AddChatGroupParticipant(AddChatGroupParticipantRequest request);
        Task<OkResponse> RemoveChatGroupParticipant(RemoveChatGroupParticipantRequest request);
        Task<OkResponse> QuitChatGroup(QuitChatGroupRequest request);
        Task<IEnumerable<ChatGroupMessageResponse>> GetChatGroupMessages(GetChatGroupMessagesRequest request);
        Task<OkResponse> SendChatGroupMessage(SendChatGroupMessageRequest request);
        Task<OkResponse> DeleteChatGroupMessage(DeleteChatGroupMessageRequest request);
    }

    public class MessagingService : IMessagingService
    {
        private readonly ConduitContext _context;
        private readonly IMapper _mapper;
        private readonly IHubContext<StreamingHub> _streaming;

        public MessagingService(ConduitContext context, IMapper mapper, IHubContext<StreamingHub> streaming)
        {
            _context = context;
            _mapper = mapper;
            _streaming = streaming;
        }

        public async Task<IEnumerable<MessageResponse>> GetMessages(GetMessagesRequest request)
        {
            var messages = await _context.Messages.AsNoTracking()
                .Where(m => request.LoggedUserID == m.SenderID && request.ParticipantID == m.RecipientID ||
                request.LoggedUserID == m.RecipientID && request.ParticipantID == m.SenderID)
                .OrderBy(m => m.SentAt)
                .Include(m => m.Sender)
                .Include(m => m.Recipient)
                .Include(m => m.SourceMessage)
                .Skip(request.PageIndex)
                .Take(request.PageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<MessageResponse>>(messages, opt =>
            {
                opt.Items["UserID"] = request.LoggedUserID;
                opt.Items["ActiveClientsUserIDs"] = new List<Guid>();
            });
        }

        public async Task<OkResponse> ReadMessages(ReadMessagesRequest request)
        {
            var messages = await _context.Messages
                .Where(m => m.SenderID == request.ParticipantID && m.RecipientID == request.LoggedUserID && m.ReadAt == null)
                .ToListAsync();

            messages.ConvertAll(m => m.ReadAt = DateTime.UtcNow);

            await _context.SaveChangesAsync();

            await NotifyReadMessage(request.LoggedUserID, request.ParticipantID);

            return new OkResponse("All messages were read.");
        }

        public async Task<OkResponse> SendMessage(SendMessageRequest request)
        {
            if (request.Content.Equals(AppParameters.DeletedMessage))
            {
                throw new Exception("You cannot create message with reserved keywords.");
            }

            if (request.RecipientID == request.SenderID)
            {
                throw new Exception("You cannot send message to yourself.");
            }

            if (request.SourceMessageID != null)
            {
                var sourceMessage = await _context.Messages.AsNoTracking()
                .Where(m => m.ID == request.SourceMessageID)
                .SingleOrDefaultAsync();

                if (sourceMessage == null || sourceMessage.SenderID != request.RecipientID && sourceMessage.SenderID != request.SenderID)
                {
                    throw new Exception("Source message not found.");
                }
            }

            var message = new Message
            {
                Content = request.Content,
                SenderID = request.SenderID,
                RecipientID = request.RecipientID,
                SentAt = DateTime.UtcNow,
                SourceMessageID = request.SourceMessageID
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            await NotifyReceiveMessage(message.SenderID, message.RecipientID);

            return new OkResponse("Message was sent.");
        }

        public async Task<OkResponse> DeleteMessage(DeleteMessageRequest request)
        {
            var message = await _context.Messages
                .Where(m => m.ID == request.MessageID)
                .SingleOrDefaultAsync();

            if (message == null)
            {
                throw new Exception("Message not found.");
            }

            if (message.Content == AppParameters.DeletedMessage)
            {
                throw new Exception("This message was already deleted.");
            }

            if (message.SenderID != request.LoggedUserID)
            {
                throw new Exception("You cannot delete someone else's message.");
            }

            message.Content = AppParameters.DeletedMessage;
            await _context.SaveChangesAsync();

            await NotifyDeleteMessage(message.SenderID, message.RecipientID, message.ID);

            return new OkResponse("Message was deleted.");
        }

        public async Task<IEnumerable<ChatGroupResponse>> GetChatGroups(GetChatGroupsRequest request)
        {
            var chatGroups = await _context.ChatGroups.AsNoTracking()
                .Where(g => g.Participants.Any(p => p.ParticipantID == request.LoggedUserID && p.EndDate == null))
                .OrderBy(g => g.Description)
                .Include(g => g.Creator)
                .Include(g => g.Rules)
                .Include(g => g.Participants)
                .ThenInclude(p => p.Participant)
                .Skip(request.PageIndex)
                .Take(request.PageSize)
                .ToListAsync();

            var activeClientsUserIDs = await _context.SignalRClients.AsNoTracking()
                .Select(c => c.UserID)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ChatGroupResponse>>(chatGroups, opt =>
            {
                opt.Items["ActiveClientsUserIDs"] = activeClientsUserIDs;
            });
        }

        public async Task<ChatGroupResponse> GetChatGroupByID(GetChatGroupByIDRequest request)
        {
            var chatGroup = await _context.ChatGroups.AsNoTracking()
                .Where(g => g.ID == request.ChatGroupID && g.Participants.Any(p => p.ParticipantID == request.LoggedUserID && p.EndDate == null))
                .Include(g => g.Creator)
                .Include(g => g.Rules)
                .Include(g => g.Participants)
                .ThenInclude(p => p.Participant)
                .SingleOrDefaultAsync();

            if (chatGroup == null)
            {
                throw new Exception("Chat group not found.");
            }

            var activeClientsUserIDs = await _context.SignalRClients.AsNoTracking()
                .Where(c => chatGroup.Participants.Select(p => p.ParticipantID).Contains(c.UserID))
                .Select(c => c.UserID)
                .ToListAsync();

            return _mapper.Map<ChatGroupResponse>(chatGroup, opt =>
            {
                opt.Items["ActiveClientsUserIDs"] = activeClientsUserIDs;
            });
        }

        public async Task<OkResponse> CreateChatGroup(CreateChatGroupRequest request)
        {
            var userChatGroups = await _context.ChatGroups.AsNoTracking()
                .Where(g => g.CreatorID == request.LoggedUserID)
                .ToListAsync();

            if (userChatGroups.Count >= 5)
            {
                throw new Exception("You can only create up to 5 groups.");
            }

            var chatGroup = new ChatGroup
            {
                Description = request.Description,
                CreatorID = request.LoggedUserID,
                Rules = new List<ChatGroupRule>()
                {
                    new ChatGroupRule(ChatGroupRuleList.AllowSendingMessages),
                    new ChatGroupRule(ChatGroupRuleList.AllowMakingCalls),
                    new ChatGroupRule(ChatGroupRuleList.AllowEditingDescription),
                }
            };

            _context.ChatGroups.Add(chatGroup);
            await _context.SaveChangesAsync();

            var loggedUser = await _context.Users.AsNoTracking()
                .Where(u => u.ID == request.LoggedUserID)
                .SingleOrDefaultAsync();

            chatGroup = await _context.ChatGroups
                .Where(g => g.ID == chatGroup.ID)
                .Include(g => g.Participants)
                .SingleOrDefaultAsync();

            if (loggedUser != null && chatGroup != null)
            {
                chatGroup.Participants.Add(new ChatGroupParticipant(chatGroup.ID, loggedUser.ID, chatGroup.CreatorID == loggedUser.ID));
                await _context.SaveChangesAsync();
            }

            return new OkResponse("Chat group created.");
        }

        public async Task<OkResponse> UpdateChatGroup(UpdateChatGroupRequest request)
        {
            var chatGroup = await _context.ChatGroups
                .Where(g => g.ID == request.ChatGroupID)
                .SingleOrDefaultAsync();

            if (chatGroup == null)
            {
                throw new Exception("Chat group not found.");
            }

            if (chatGroup.CreatorID != request.LoggedUserID && !chatGroup.Rules.Any(x => x.Rule == (int)ChatGroupRuleList.AllowEditingDescription))
            {
                throw new Exception("You cannot modify this chat group.");
            }

            chatGroup.Description = request.Description;

            await _context.SaveChangesAsync();

            return new OkResponse("Chat group updated.");
        }

        public async Task<OkResponse> AddChatGroupParticipant(AddChatGroupParticipantRequest request)
        {
            var chatGroup = await _context.ChatGroups
                .Where(g => g.ID == request.ChatGroupID)
                .Include(g => g.Participants)
                .SingleOrDefaultAsync();

            if (chatGroup == null)
            {
                throw new Exception("Chat group not found.");
            }

            if (chatGroup.CreatorID != request.LoggedUserID)
            {
                throw new Exception("You cannot modify this chat group.");
            }

            if (chatGroup.Participants.Any(p => p.ParticipantID == request.ParticipantID))
            {
                throw new Exception("Participant is already in this chat group.");
            }

            if (chatGroup.Participants.Count >= AppParameters.ChatGroupMaximumCapacity)
            {
                throw new Exception($"This chat group reached the maximum capacity ({AppParameters.ChatGroupMaximumCapacity}).");
            }

            var participant = await _context.Users
                .Where(u => u.ID == request.ParticipantID)
                .SingleOrDefaultAsync();

            if (participant == null)
            {
                throw new Exception("Participant not found.");
            }

            chatGroup.Participants.Add(new ChatGroupParticipant(chatGroup.ID, participant.ID, false));

            await _context.SaveChangesAsync();

            return new OkResponse("Participant added.");
        }

        public async Task<OkResponse> RemoveChatGroupParticipant(RemoveChatGroupParticipantRequest request)
        {
            var chatGroup = await _context.ChatGroups
                .Where(g => g.ID == request.ChatGroupID)
                .Include(g => g.Participants)
                .SingleOrDefaultAsync();

            if (chatGroup == null)
            {
                throw new Exception("Chat group not found.");
            }

            if (chatGroup.CreatorID != request.LoggedUserID)
            {
                throw new Exception("You cannot modify this chat group.");
            }

            if (!chatGroup.Participants.Any(p => p.ParticipantID == request.ParticipantID))
            {
                throw new Exception("Participant not found.");
            }

            var participant = chatGroup.Participants
                .Where(p => p.ParticipantID == request.ParticipantID)
                .SingleOrDefault();

            if (participant == null)
            {
                throw new Exception("Participant not found.");
            }

            participant.EndDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new OkResponse("Participant removed.");
        }

        public async Task<OkResponse> QuitChatGroup(QuitChatGroupRequest request)
        {
            var chatGroup = await _context.ChatGroups
                .Where(g => g.ID == request.ChatGroupID && g.Participants.Select(p => p.ParticipantID).Contains(request.LoggedUserID))
                .Include(g => g.Participants)
                .SingleOrDefaultAsync();

            if (chatGroup == null)
            {
                throw new Exception("Chat group not found.");
            }

            var participant = chatGroup.Participants
                .Where(p => p.ParticipantID == request.LoggedUserID)
                .SingleOrDefault();

            if (participant == null)
            {
                throw new Exception("Participant not found.");
            }

            participant.EndDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            if (!chatGroup.Participants.Any(p => p.IsActive))
            {
                _context.ChatGroups.Remove(chatGroup);

                await _context.SaveChangesAsync();
            }

            return new OkResponse("You quit this group.");
        }

        public async Task<IEnumerable<ChatGroupMessageResponse>> GetChatGroupMessages(GetChatGroupMessagesRequest request)
        {
            var messages = await _context.ChatGroupMessages.AsNoTracking()
                .Where(m => m.ChatGroupID == request.ChatGroupID &&
                m.ChatGroup.Participants.Select(p => p.ParticipantID).Contains(request.LoggedUserID))
                .OrderBy(m => m.SentAt)
                .Include(m => m.Sender)
                .Include(m => m.SourceMessage)
                .ThenInclude(m => m.Sender)
                .Skip(request.PageIndex)
                .Take(request.PageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ChatGroupMessageResponse>>(messages, opt =>
            {
                opt.Items["ActiveClientsUserIDs"] = new List<Guid>();
            });
        }

        public async Task<OkResponse> SendChatGroupMessage(SendChatGroupMessageRequest request)
        {
            var chatGroup = await _context.ChatGroups.AsNoTracking()
                .Where(g => g.ID == request.ChatGroupID && g.Participants.Select(p => p.ParticipantID).Contains(request.SenderID))
                .Include(g => g.Participants)
                .SingleOrDefaultAsync();

            if (chatGroup == null)
            {
                throw new Exception("Chat group not found.");
            }

            if (request.Content.Equals(AppParameters.DeletedMessage))
            {
                throw new Exception("You cannot create message with reserved keywords.");
            }

            var message = new ChatGroupMessage
            {
                Content = request.Content,
                SenderID = request.SenderID,
                ChatGroupID = request.ChatGroupID,
                SentAt = DateTime.UtcNow,
                SourceMessageID = request.SourceMessageID
            };

            _context.ChatGroupMessages.Add(message);
            await _context.SaveChangesAsync();

            await NotifyReceiveChatGroupMessage(chatGroup.ID, chatGroup.Participants.Select(c => c.ParticipantID).ToArray());

            return new OkResponse("Message was sent.");
        }

        public async Task<OkResponse> DeleteChatGroupMessage(DeleteChatGroupMessageRequest request)
        {
            var message = await _context.ChatGroupMessages
                .Where(m => m.ID == request.MessageID)
                .Include(m => m.ChatGroup)
                .ThenInclude(g => g.Participants)
                .SingleOrDefaultAsync();

            if (message == null)
            {
                throw new Exception("Message not found.");
            }

            if (message.Content == AppParameters.DeletedMessage)
            {
                throw new Exception("This message was already deleted.");
            }

            if (message.SenderID != request.LoggedUserID)
            {
                throw new Exception("You cannot delete someone else's message.");
            }

            message.Content = AppParameters.DeletedMessage;
            await _context.SaveChangesAsync();

            await NotifyDeleteChatGroupMessage(message.ChatGroupID, message.ChatGroup.Participants.Select(p => p.ParticipantID).ToArray(), message.ID);

            return new OkResponse("Message was deleted.");
        }

        private async Task NotifyReceiveMessage(Guid senderID, Guid recipientID)
        {
            var clients = await _context.SignalRClients.AsNoTracking()
                .Where(c => c.UserID == senderID || c.UserID == recipientID)
                .Select(c => c.ConnectionID)
                .ToListAsync();

            await _streaming.Clients.Clients(clients).SendAsync("ReceiveMessage", new
            {
                SenderID = senderID,
                RecipientID = recipientID
            });
        }

        private async Task NotifyReadMessage(Guid senderID, Guid recipientID)
        {
            var clients = await _context.SignalRClients.AsNoTracking()
                .Where(c => c.UserID == senderID || c.UserID == recipientID)
                .Select(c => c.ConnectionID)
                .ToListAsync();

            await _streaming.Clients.Clients(clients).SendAsync("ReadMessage", new
            {
                SenderID = senderID,
                RecipientID = recipientID
            });
        }

        private async Task NotifyDeleteMessage(Guid senderID, Guid recipientID, int messageID)
        {
            var clients = await _context.SignalRClients.AsNoTracking()
                .Where(c => c.UserID == senderID || c.UserID == recipientID)
                .Select(c => c.ConnectionID)
                .ToListAsync();

            await _streaming.Clients.Clients(clients).SendAsync("DeleteMessage", new
            {
                SenderID = senderID,
                RecipientID = recipientID,
                MessageID = messageID
            });
        }

        private async Task NotifyReceiveChatGroupMessage(Guid chatGroupID, Guid[] participants)
        {
            var clients = await _context.SignalRClients.AsNoTracking()
                .Where(c => participants.Contains(c.UserID))
                .Select(c => c.ConnectionID)
                .ToListAsync();

            await _streaming.Clients.Clients(clients).SendAsync("ReceiveChatGroupMessage", new
            {
                ChatGroupID = chatGroupID,
                Participants = participants
            });
        }

        private async Task NotifyDeleteChatGroupMessage(Guid chatGroupID, Guid[] participants, int messageID)
        {
            var clients = await _context.SignalRClients.AsNoTracking()
                .Where(c => participants.Contains(c.UserID))
                .Select(c => c.ConnectionID)
                .ToListAsync();

            await _streaming.Clients.Clients(clients).SendAsync("DeleteChatGroupMessage", new
            {
                ChatGroupID = chatGroupID,
                Participants = participants,
                MessageID = messageID
            });
        }
    }
}
