using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Model;
using Model.DataModels;
using Services.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Services.SignalR
{
    public class StreamingHub : Hub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ConduitContext _context;
        private readonly AppSettings _appSettings;

        public StreamingHub(IHttpContextAccessor httpContextAccessor, ConduitContext context, IOptions<AppSettings> appSettings)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _appSettings = appSettings.Value;
        }

        public override async Task<Task> OnConnectedAsync()
        {
            QueryHelpers.ParseQuery(_httpContextAccessor.HttpContext.Request.QueryString.Value).TryGetValue("token", out var token);
            var userID = GetUserID(token);

            _context.SignalRClients.Add(new SignalRClient
            {
                UserID = userID,
                ConnectionID = Context.ConnectionId
            });

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
            QueryHelpers.ParseQuery(_httpContextAccessor.HttpContext.Request.QueryString.Value).TryGetValue("token", out var token);
            var userID = GetUserID(token);

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
        public async Task Typing(int senderID, int recipientID, bool isTyping)
        {
            var clients = await _context.SignalRClients.AsNoTracking()
                .Where(c => c.UserID == senderID || c.UserID == recipientID)
                .Select(c => c.ConnectionID)
                .ToListAsync();

            await Clients.Clients(clients).SendAsync("Typing", new { SenderID = senderID, RecipientID = recipientID, IsTyping = isTyping });
        }

        [HubMethodName("BeginCall")]
        public async Task BeginCall(int chatID, string type)
        {
            var group = await _context.ChatGroups.AsNoTracking()
                .Where(g => g.ID == chatID)
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
                .Where(c => group.Participants.Select(p => p.ID).Contains(c.UserID) && c.UserID != callerID)
                .Select(c => c.ConnectionID)
                .ToListAsync();

            await Clients.Clients(clients).SendAsync("ReceivingCall", new { ChatID = chatID, CallerConnectionID = Context.ConnectionId, Type = type });
        }

        [HubMethodName("CancelCall")]
        public async Task CancelCall(int chatID)
        {
            var group = await _context.ChatGroups.AsNoTracking()
                .Where(g => g.ID == chatID)
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
                .Where(c => group.Participants.Select(p => p.ID).Contains(c.UserID))
                .Select(c => c.ConnectionID)
                .ToListAsync();

            await Clients.Clients(clients).SendAsync("CallCancelled", new { ChatID = chatID });
        }

        [HubMethodName("AcceptCall")]
        public async Task AcceptCall(int chatID, string callerConnectionID, string type)
        {
            var room = chatID.ToString();

            await Groups.AddToGroupAsync(Context.ConnectionId, room);
            await Groups.AddToGroupAsync(callerConnectionID, room);

            await Clients.Group(room).SendAsync("CallAccepted", new { ChatID = chatID, Type = type });
        }

        [HubMethodName("DenyCall")]
        public async Task DenyCall(int chatID, string callerConnectionID)
        {
            await Clients.Clients(callerConnectionID).SendAsync("CallDenied", new { ChatID = chatID });
        }

        [HubMethodName("EndCall")]
        public async Task EndCall(int chatID)
        {
            var room = chatID.ToString();

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, room);
            await Clients.Group(room).SendAsync("CallEnded", new { ChatID = chatID });
        }

        [HubMethodName("SendAudio")]
        public async Task SendAudio(int chatID, dynamic audioData)
        {
            var room = chatID.ToString();

            await Clients.Group(room).SendAsync("ReceiveAudio", new { ChatID = chatID, Audio = audioData });
        }

        private int GetUserID(string? token)
        {
            if (token == null)
            {
                throw new Exception("Authentication is required");
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                return int.Parse(jwtToken.Claims.First(x => x.Type == "userID").Value);
            }
            catch
            {
                throw new Exception("Authentication is required");
            }
        }
    }
}
