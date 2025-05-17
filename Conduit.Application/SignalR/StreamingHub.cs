using Conduit.Infrastructure.Data;
using Conduit.Application.Helpers;
using Conduit.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Conduit.Application.SignalR
{
    public class StreamingHub : Hub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ConduitContext _context;

        public StreamingHub(IHttpContextAccessor httpContextAccessor, ConduitContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public override async Task<Task> OnConnectedAsync()
        {
            var userID = GetUserID();

            _context.SignalRClients.Add(new SignalRClient(Context.ConnectionId, userID));

            await _context.SaveChangesAsync();

            var user = await _context.Users.AsNoTracking()
                .Where(u => u.ID == userID)
                .SingleOrDefaultAsync();

            if (user != null)
            {
                await Clients.All.SendAsync("Connecting", new
                {
                    SenderID = userID,
                    LastSeen = "Online"
                });
            }

            return base.OnConnectedAsync();
        }

        public override async Task<Task> OnDisconnectedAsync(Exception exception)
        {
            var userID = GetUserID();

            var client = await _context.SignalRClients
                .Where(c => c.ConnectionID == Context.ConnectionId)
                .SingleOrDefaultAsync();

            if (client != null)
            {
                _context.SignalRClients.Remove(client);
            }

            var clients = await _context.SignalRClients.AsNoTracking()
                .Where(c => c.UserID == userID)
                .ToListAsync();

            var user = await _context.Users
                .Where(u => u.ID == userID)
                .SingleOrDefaultAsync();

            if (user != null && !clients.Any(c => c.ConnectionID != Context.ConnectionId))
            {
                var lastSeen = DateTime.UtcNow;
                user.LastSeen = lastSeen;

                await Clients.All.SendAsync("Disconnecting", new
                {
                    SenderID = userID,
                    LastSeen = Extensions.GetLastSeen(lastSeen, false)
                });
            }

            await _context.SaveChangesAsync();

            return base.OnDisconnectedAsync(exception);
        }

        [HubMethodName("Typing")]
        public async Task Typing(Guid senderID, Guid recipientID, bool isTyping)
        {
            var clients = await _context.SignalRClients.AsNoTracking()
                .Where(c => c.UserID == senderID || c.UserID == recipientID)
                .Select(c => c.ConnectionID)
                .ToListAsync();

            await Clients.Clients(clients).SendAsync("Typing", new { SenderID = senderID, RecipientID = recipientID, IsTyping = isTyping });
        }

        [HubMethodName("BeginCall")]
        public async Task BeginCall(Guid chatGroupID, string type)
        {
            var group = await _context.ChatGroups.AsNoTracking()
                .Where(g => g.ID == chatGroupID)
                .Include(g => g.Participants)
                .SingleOrDefaultAsync();

            if (group == null)
            {
                throw new Exception("Chat not found");
            }

            var callerID = await _context.SignalRClients.AsNoTracking()
                .Where(c => c.ConnectionID == Context.ConnectionId)
                .Select(c => c.UserID)
                .SingleOrDefaultAsync();

            if (callerID == default)
            {
                throw new Exception("Caller not found");
            }

            var clients = await _context.SignalRClients.AsNoTracking()
                .Where(c => group.Participants.Select(p => p.ParticipantID).Contains(c.UserID) && c.UserID != callerID)
                .Select(c => c.ConnectionID)
                .ToListAsync();

            await Clients.Clients(clients).SendAsync("ReceivingCall", new { ChatGroupID = chatGroupID, CallerConnectionID = Context.ConnectionId, Type = type });
        }

        [HubMethodName("CancelCall")]
        public async Task CancelCall(Guid chatGroupID)
        {
            var group = await _context.ChatGroups.AsNoTracking()
                .Where(g => g.ID == chatGroupID)
                .Include(g => g.Participants)
                .SingleOrDefaultAsync();

            if (group == null)
            {
                throw new Exception("Chat not found");
            }

            var caller = await _context.SignalRClients.AsNoTracking()
                .Where(u => u.ConnectionID == Context.ConnectionId)
                .SingleOrDefaultAsync();

            if (caller == null)
            {
                throw new Exception("Caller not found");
            }

            var clients = await _context.SignalRClients.AsNoTracking()
                .Where(c => group.Participants.Select(p => p.ParticipantID).Contains(c.UserID))
                .Select(c => c.ConnectionID)
                .ToListAsync();

            await Clients.Clients(clients).SendAsync("CallCancelled", new { ChatGroupID = chatGroupID });
        }

        [HubMethodName("AcceptCall")]
        public async Task AcceptCall(Guid chatGroupID, string callerConnectionID, string type)
        {
            var room = GetRoomID(chatGroupID);

            await Groups.AddToGroupAsync(Context.ConnectionId, room);
            await Groups.AddToGroupAsync(callerConnectionID, room);

            await Clients.Group(room).SendAsync("CallAccepted", new { ChatGroupID = chatGroupID, Type = type });
        }

        [HubMethodName("DenyCall")]
        public async Task DenyCall(Guid chatGroupID, string callerConnectionID)
        {
            await Clients.Clients(callerConnectionID).SendAsync("CallDenied", new { ChatGroupID = chatGroupID });
        }

        [HubMethodName("EndCall")]
        public async Task EndCall(Guid chatGroupID)
        {
            var room = GetRoomID(chatGroupID);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, room);
            await Clients.Group(room).SendAsync("CallEnded", new { ChatGroupID = chatGroupID });
        }

        [HubMethodName("SendAudio")]
        public async Task SendAudio(Guid chatGroupID, dynamic audioData)
        {
            var room = GetRoomID(chatGroupID);

            await Clients.Group(room).SendAsync("ReceiveAudio", new { ChatGroupID = chatGroupID, Audio = audioData });
        }

        private Guid GetUserID()
        {
            QueryHelpers.ParseQuery(_httpContextAccessor.HttpContext.Request.QueryString.Value).TryGetValue("token", out var token);

            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("Authentication is required");
            }

            try
            {
                var jwtSecret = EnvironmentVariables.GetEnvironmentVariable(VariableType.JwtSecret);
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(jwtSecret);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                return Guid.Parse(jwtToken.Claims.First(x => x.Type == "userID").Value);
            }
            catch (Exception)
            {
                throw new Exception("Authentication is required");
            }
        }

        private string GetRoomID(Guid chatGroupID)
        {
            return chatGroupID.ToString();
        }
    }
}
