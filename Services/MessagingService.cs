using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Commands.Requests.Post;
using Model.Commands.Requests.Delete;
using Model.Commands.Requests.Put;
using Model.Commands.Responses;
using Model.DataModels;
using Model.Queries.Requests;
using Model.Queries.Responses;
using Services.Helpers;
using Services.SignalR;

namespace Services
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
                .Where(m => (request.UserID == m.SenderID && request.ParticipantID == m.RecipientID) ||
                (request.UserID == m.RecipientID && request.ParticipantID == m.SenderID))
                .OrderBy(m => m.SentAt)
                .Include(m => m.Sender)
                .Include(m => m.Recipient)
                .Include(m => m.SourceMessage)
                .Skip(request.PaginationIndex)
                .Take(request.PaginationSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<MessageResponse>>(messages, opt =>
            {
                opt.Items["UserID"] = request.UserID;
                opt.Items["ActiveClientsUserIDs"] = new List<int>();
            });
        }

        public async Task<OkResponse> ReadMessages(ReadMessagesRequest request)
        {
            var messages = await _context.Messages
                .Where(m => m.SenderID == request.ParticipantID && m.RecipientID == request.UserID && m.ReadAt == null)
                .ToListAsync();

            messages.ConvertAll(m => m.ReadAt = DateTime.UtcNow);

            await _context.SaveChangesAsync();

            await NotifyReadMessage(request.UserID, request.ParticipantID);

            return new OkResponse("All messages were read.");
        }

        public async Task<OkResponse> SendMessage(SendMessageRequest request)
        {
            if (request.Content.Equals(AppParameters.DeletedMessage))
            {
                throw new Exception("Cannot create message with reserved keywords.");
            }

            if (request.RecipientID == request.SenderID)
            {
                throw new Exception("Cannot send message to yourself.");
            }

            if (request.SourceMessageID != null)
            {
                var sourceMessage = await _context.Messages.AsNoTracking()
                .Where(m => m.ID == request.SourceMessageID)
                .SingleOrDefaultAsync();

                if (sourceMessage == null || (sourceMessage.SenderID != request.RecipientID && sourceMessage.SenderID != request.SenderID))
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

            if (message.SenderID != request.UserID)
            {
                throw new Exception("Cannot delete someone else's message.");
            }

            message.Content = AppParameters.DeletedMessage;
            await _context.SaveChangesAsync();

            await NotifyDeleteMessage(message.SenderID, message.RecipientID, message.ID);

            return new OkResponse("Message was deleted.");
        }

        public async Task<IEnumerable<ChatGroupResponse>> GetChatGroups(GetChatGroupsRequest request)
        {
            var chatGroups = await _context.ChatGroups.AsNoTracking()
                .Where(g => g.Participants.Select(p => p.ID).Contains(request.UserID))
                .Include(g => g.Participants)
                .OrderBy(g => g.Description)
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
                .Where(g => g.ID == request.ChatGroupID && g.Participants.Select(p => p.ID).Contains(request.UserID))
                .Include(g => g.Participants)
                .SingleOrDefaultAsync();

            if (chatGroup == null)
            {
                throw new Exception("Chat group not found.");
            }

            var activeClientsUserIDs = await _context.SignalRClients.AsNoTracking()
                .Where(c => chatGroup.Participants.Select(p => p.ID).Contains(c.UserID))
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
                .Where(g => g.CreatedBy == request.UserID)
                .ToListAsync();

            if (userChatGroups.Count >= 5)
            {
                throw new Exception("You can only create up to 5 groups.");
            }

            var chatGroup = new ChatGroup
            {
                Description = request.Description,
                CreatedBy = request.UserID
            };

            _context.ChatGroups.Add(chatGroup);
            await _context.SaveChangesAsync();

            var user = await _context.Users.AsNoTracking()
                .Where(u => u.ID == request.UserID)
                .SingleOrDefaultAsync();

            chatGroup = await _context.ChatGroups
                .Where(g => g.ID == chatGroup.ID)
                .Include(g => g.Participants)
                .SingleOrDefaultAsync();

            if (user != null && chatGroup != null)
            {
                chatGroup.Participants.Add(user);
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

            if (chatGroup.CreatedBy != request.UserID)
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

            if (chatGroup.CreatedBy != request.UserID)
            {
                throw new Exception("You cannot modify this chat group.");
            }

            if (chatGroup.Participants.Any(p => p.ID == request.ParticipantID))
            {
                throw new Exception("Participant is already in this chat group.");
            }

            if (chatGroup.Participants.Count >= 50)
            {
                throw new Exception("This chat group reached the maximum capacity (50).");
            }

            var participant = await _context.Users
                .Where(u => u.ID == request.ParticipantID)
                .SingleOrDefaultAsync();

            if (participant == null)
            {
                throw new Exception("Participant not found.");
            }

            chatGroup.Participants.Add(participant);

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

            if (chatGroup.CreatedBy != request.UserID)
            {
                throw new Exception("You cannot modify this chat group.");
            }

            if (!chatGroup.Participants.Any(p => p.ID == request.ParticipantID))
            {
                throw new Exception("Participant not found.");
            }

            var participant = await _context.Users
                .Where(u => u.ID == request.ParticipantID)
                .SingleOrDefaultAsync();

            if (participant == null)
            {
                throw new Exception("Participant not found.");
            }

            chatGroup.Participants.Remove(participant);

            await _context.SaveChangesAsync();

            return new OkResponse("Participant removed.");
        }

        public async Task<OkResponse> QuitChatGroup(QuitChatGroupRequest request)
        {
            var chatGroup = await _context.ChatGroups
                .Where(g => g.ID == request.ChatGroupID && g.Participants.Select(p => p.ID).Contains(request.UserID))
                .Include(g => g.Participants)
                .SingleOrDefaultAsync();

            if (chatGroup == null)
            {
                throw new Exception("Chat group not found.");
            }

            var participant = chatGroup.Participants
                .Where(p => p.ID == request.UserID)
                .SingleOrDefault();

            if (participant == null)
            {
                throw new Exception("Participant not found.");
            }

            chatGroup.Participants.Remove(participant);

            if (!chatGroup.Participants.Any())
            {
                _context.ChatGroups.Remove(chatGroup);
            }

            await _context.SaveChangesAsync();

            return new OkResponse("You quit this group.");
        }

        public async Task<IEnumerable<ChatGroupMessageResponse>> GetChatGroupMessages(GetChatGroupMessagesRequest request)
        {
            var messages = await _context.ChatGroupMessages.AsNoTracking()
                .Where(m => m.ChatGroupID == request.ChatGroupID &&
                m.ChatGroup.Participants.Select(p => p.ID).Contains(request.UserID))
                .OrderBy(m => m.SentAt)
                .Include(m => m.Sender)
                .Include(m => m.SourceMessage)
                .ThenInclude(m => m.Sender)
                .Skip(request.PaginationIndex)
                .Take(request.PaginationSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ChatGroupMessageResponse>>(messages, opt =>
            {
                opt.Items["ActiveClientsUserIDs"] = new List<int>();
            });
        }

        public async Task<OkResponse> SendChatGroupMessage(SendChatGroupMessageRequest request)
        {
            var chatGroup = await _context.ChatGroups.AsNoTracking()
                .Where(g => g.ID == request.ChatGroupID && g.Participants.Select(p => p.ID).Contains(request.SenderID))
                .Include(g => g.Participants)
                .SingleOrDefaultAsync();

            if (chatGroup == null)
            {
                throw new Exception("Chat group not found.");
            }

            if (request.Content.Equals(AppParameters.DeletedMessage))
            {
                throw new Exception("Cannot create message with reserved keywords.");
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

            await NotifyReceiveChatGroupMessage(chatGroup.ID, chatGroup.Participants.Select(c => c.ID).ToArray());

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

            if (message.SenderID != request.UserID)
            {
                throw new Exception("Cannot delete someone else's message.");
            }

            message.Content = AppParameters.DeletedMessage;
            await _context.SaveChangesAsync();

            await NotifyDeleteChatGroupMessage(message.ChatGroupID, message.ChatGroup.Participants.Select(p => p.ID).ToArray(), message.ID);

            return new OkResponse("Message was deleted.");
        }

        private async Task NotifyReceiveMessage(int senderID, int recipientID)
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

        private async Task NotifyReadMessage(int senderID, int recipientID)
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

        private async Task NotifyDeleteMessage(int senderID, int recipientID, int messageID)
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

        private async Task NotifyReceiveChatGroupMessage(int chatGroupID, int[] participants)
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

        private async Task NotifyDeleteChatGroupMessage(int chatGroupID, int[] participants, int messageID)
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
