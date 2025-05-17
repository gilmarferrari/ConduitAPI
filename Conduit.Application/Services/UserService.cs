using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Conduit.Application.Commands.Responses;
using Conduit.Application.Queries.Requests;
using Conduit.Application.Queries.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;
using Conduit.Domain.Entities;
using Conduit.Application.Commands.Requests;
using Conduit.Infrastructure.Data;
using Conduit.Application.Helpers;

namespace Conduit.Application.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponse>> GetUsers(GetUsersRequest request);
        Task<UserResponse> GetUserByID(GetUserByIDRequest request);
        Task<OkResponse> UpdateUser(UpdateUserRequest request);
        Task<AuthenticationResponse> Authenticate(AuthenticateRequest request);
        Task<AuthenticationResponse> RefreshToken(RefreshTokenRequest request);
        Task<OkResponse> RevokeToken(RevokeTokenRequest request);
        Task<OkResponse> ResetPassword(ResetPasswordRequest request);
        Task<OkResponse> ResendCode(ResendCodeRequest request);
    }

    public class UserService : IUserService
    {
        private readonly ConduitContext _context;
        private readonly IMapper _mapper;

        public UserService(ConduitContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserResponse>> GetUsers(GetUsersRequest request)
        {
            var users = await _context.Users.AsNoTracking()
                .Where(u => !request.ActiveOnly || u.UserAccount.IsActive)
                .Include(u => u.UserAccount)
                .Include(u => u.Roles)
                .Skip(request.PageIndex)
                .Take(request.PageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserResponse>>(users);
        }

        public async Task<UserResponse> GetUserByID(GetUserByIDRequest request)
        {
            var user = await _context.Users.AsNoTracking()
                .Where(u => u.ID == request.ID)
                .Include(u => u.Roles)
                .SingleOrDefaultAsync();

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            return _mapper.Map<UserResponse>(user);
        }

        public async Task<OkResponse> UpdateUser(UpdateUserRequest request)
        {
            var user = await _context.Users.AsNoTracking()
                .Where(u => u.ID == request.ID)
                .SingleOrDefaultAsync();

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            user.Name = request.Name;

            foreach (var existingRole in user.Roles.ToList())
            {
                if (!request.Roles.Any(r => r == existingRole.Role))
                {
                    user.Roles.Remove(existingRole);
                }
            }

            foreach (var role in request.Roles)
            {
                var userRole = user.Roles
                    .Where(r => r.Role == role)
                    .SingleOrDefault();

                if (userRole == null)
                {
                    user.Roles.Add(new UserRole(user.ID, role));
                }
            }

            await _context.SaveChangesAsync();

            return new OkResponse("User updated");
        }

        public async Task<AuthenticationResponse> Authenticate(AuthenticateRequest request)
        {
            var user = await _context.Users
                .Where(u => u.UserAccount.Email.Equals(request.Email))
                .Include(u => u.UserAccount)
                .Include(u => u.LongLivedTokens)
                .Include(u => u.Roles)
                .SingleOrDefaultAsync();

            if (user == null || !BC.Verify(request.Password, user.UserAccount.PasswordHash))
            {
                throw new Exception("Invalid e-mail or password.");
            }

            if (!user.UserAccount.IsActive)
            {
                throw new Exception("Account is not active.");
            }

            if (!user.UserAccount.IsAccountActivated)
            {
                throw new Exception("Account was not activated.");
            }

            CreateUserActivityLog(user.ID, "User performed authentication", request.IpAddress, request.Agent, request.Origin);

            var token = GenerateJwtToken(user.ID, request.IpAddress, request.Agent, request.Origin);

            var longLivedToken = GenerateLongLivedToken(user.ID);
            _context.LongLivedTokens.Add(longLivedToken);

            RemoveOldRefreshTokens(user);

            await _context.SaveChangesAsync();

            return new AuthenticationResponse
            {
                UserID = user.ID,
                JWT = token,
                LongLivedToken = longLivedToken.Token,
                Roles = user.Roles.Select(x => ((RoleList)x.Role).ToString()).ToArray(),
            };
        }

        public async Task<AuthenticationResponse> RefreshToken(RefreshTokenRequest request)
        {
            var (longLivedToken, user) = await GetLongLivedToken(request.LongLivedToken);

            var newLongLivedToken = GenerateLongLivedToken(user.ID);
            longLivedToken.RevokedAt = DateTime.UtcNow;
            _context.LongLivedTokens.Add(newLongLivedToken);

            CreateUserActivityLog(user.ID, $"Replaced long lived token '{longLivedToken.Token}' by '{newLongLivedToken.Token}'", request.IpAddress, request.Agent, request.Origin);

            RemoveOldRefreshTokens(user);

            var token = GenerateJwtToken(user.ID, request.IpAddress, request.Agent, request.Origin);

            await _context.SaveChangesAsync();

            return new AuthenticationResponse
            {
                UserID = user.ID,
                JWT = token,
                LongLivedToken = newLongLivedToken.Token,
                Roles = user.Roles.Select(x => ((RoleList)x.Role).ToString()).ToArray(),
            };
        }

        public async Task<OkResponse> RevokeToken(RevokeTokenRequest request)
        {
            var (longLivedToken, user) = await GetLongLivedToken(request.LongLivedToken);
            longLivedToken.RevokedAt = DateTime.UtcNow;

            CreateUserActivityLog(user.ID, $"Revoked long lived token '{longLivedToken.Token}'", request.IpAddress, request.Agent, request.Origin);

            await _context.SaveChangesAsync();

            return new OkResponse("Token revoked");
        }

        public async Task<OkResponse> ResetPassword(ResetPasswordRequest request)
        {
            var user = await _context.Users
                .Where(u => u.UserAccount.Email.Equals(request.Email))
                .Include(u => u.UserAccount)
                .SingleOrDefaultAsync();

            if (user.UserAccount == null || user.UserAccount.ResetToken != request.ResetToken)
            {
                throw new Exception("Invalid token.");
            }

            if (user.UserAccount.ResetTokenExpiresAt < DateTime.UtcNow)
            {
                throw new Exception("Token has expired");
            }

            user.UserAccount.ResetToken = null;
            user.UserAccount.ResetTokenExpiresAt = null;
            user.UserAccount.PasswordHash = BC.HashPassword(request.Password);

            CreateUserActivityLog(user.ID, $"Account password has been reset [Token: {request.ResetToken}]", request.IpAddress, request.Agent, request.Origin);

            await _context.SaveChangesAsync();

            return new OkResponse("Password has been reset");
        }

        public async Task<OkResponse> ResendCode(ResendCodeRequest request)
        {
            var user = await _context.Users
                .Where(u => u.UserAccount.Email.Equals(request.Email))
                .Include(u => u.UserAccount)
                .SingleOrDefaultAsync();

            if (user.UserAccount != null)
            {
                var (token, expiresAt) = Extensions.GenerateRandomCode();

                switch (request.Type)
                {
                    case (int)TokenType.ActivationToken:
                        user.UserAccount.ActivationToken = token;
                        user.UserAccount.ActivationTokenExpiresAt = expiresAt;
                        break;
                    case (int)TokenType.ResetToken:
                        user.UserAccount.ResetToken = token;
                        user.UserAccount.ResetTokenExpiresAt = expiresAt;
                        break;
                }

                CreateUserActivityLog(user.ID, $"Generated new {(TokenType)request.Type} [Token: {token} | ExpiresAt: {expiresAt}]", request.IpAddress, request.Agent, request.Origin);

                await _context.SaveChangesAsync();

                // Create job to send the token to the account's e-mail
            }

            return new OkResponse("An e-mail has been sent to your e-mail address.");
        }

        private string GenerateJwtToken(Guid userID, string? IpAddress, string? agent, string? origin)
        {
            var jwtSecret = EnvironmentVariables.GetEnvironmentVariable(VariableType.JwtSecret);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("userID", userID.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private LongLivedToken GenerateLongLivedToken(Guid userID)
        {
            var token = Extensions.GenerateRandomToken();

            var longLivedToken = new LongLivedToken
            {
                Token = token,
                UserID = userID,
            };

            return longLivedToken;
        }

        private async Task<(LongLivedToken, User)> GetLongLivedToken(string token)
        {
            var user = await _context.Users
                .Where(u => u.LongLivedTokens.Any(t => t.Token == token))
                .Include(u => u.LongLivedTokens)
                .Include(u => u.Roles)
                .SingleOrDefaultAsync();

            if (user == null)
            {
                throw new Exception("Invalid token");
            }

            var longLivedToken = user.LongLivedTokens.Single(x => x.Token == token);

            if (!longLivedToken.IsActive)
            {
                throw new Exception("Invalid token");
            }

            return (longLivedToken, user);
        }

        private void RemoveOldRefreshTokens(User user)
        {
            user.LongLivedTokens.RemoveAll(x => !x.IsActive);
        }

        private void CreateUserActivityLog(Guid userID, string content, string? IpAddress, string? agent, string? origin)
        {
            var userActivityLog = new UserActivityLog
            {
                Date = DateTime.UtcNow,
                Content = content,
                IpAddress = IpAddress,
                Agent = agent,
                Origin = origin,
                UserID = userID
            };

            _context.UserActivityLogs.Add(userActivityLog);
        }
    }
}
