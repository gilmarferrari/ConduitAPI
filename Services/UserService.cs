using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Model;
using Model.Commands.Requests.Post;
using Model.Commands.Requests.Delete;
using Model.Commands.Requests.Put;
using Model.Commands.Responses;
using Model.DataModels;
using Model.Queries.Requests;
using Model.Queries.Responses;
using Services.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponse>> GetUsers(GetUsersRequest request);
        Task<UserResponse> GetUserByID(GetUserByIDRequest request);
        Task<OkResponse> UpdateUser(UpdateUserRequest request);
        Task<OkResponse> CreateUserAccount(CreateUserAccountRequest request);
        Task<OkResponse> DeleteUserAccount(DeleteUserAccountRequest request);
        Task<AuthenticationResponse> Authenticate(AuthenticateRequest request);
        Task<AuthenticationResponse> RefreshToken(RefreshTokenRequest request);
        Task<OkResponse> RevokeToken(RevokeTokenRequest request);
        Task<OkResponse> ActivateAccount(ActivateAccountRequest request);
        Task<OkResponse> ResetPassword(ResetPasswordRequest request);
        Task<OkResponse> ResendCode(ResendCodeRequest request);
    }

    public class UserService : IUserService
    {
        private readonly ConduitContext _context;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;

        public UserService(ConduitContext context, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        public async Task<IEnumerable<UserResponse>> GetUsers(GetUsersRequest request)
        {
            var users = await _context.Users.AsNoTracking()
                .Where(u => !request.ActiveOnly || u.UserAccount.IsActive)
                .Include(u => u.UserAccount)
                .Include(u => u.Roles)
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
                if (!request.RolesIDs.Any(r => r == existingRole.ID))
                {
                    user.Roles.Remove(existingRole);
                }
            }

            foreach (var selectedRole in request.RolesIDs)
            {
                var userRole = user.Roles
                    .Where(r => r.ID == selectedRole && r.ID != default(int))
                    .SingleOrDefault();

                if (userRole == null)
                {
                    var role = await _context.Roles
                        .Where(r => r.ID == selectedRole)
                        .SingleOrDefaultAsync();

                    if (role == null)
                    {
                        throw new Exception("Role not found.");
                    }

                    user.Roles.Add(role);
                }
            }

            await _context.SaveChangesAsync();

            return new OkResponse("User updated");
        }

        public async Task<OkResponse> CreateUserAccount(CreateUserAccountRequest request)
        {
            var userAccounts = await _context.UserAccounts.AsNoTracking()
                .Where(a => a.Email.Equals(request.Email))
                .SingleOrDefaultAsync();

            if (userAccounts != null)
            {
                throw new Exception("This e-mail is already taken.");
            }

            var (activationToken, expiresAt) = await GenerateRandomToken(TokenType.ActivationToken);

            var userAccount = new UserAccount
            {
                Email = request.Email,
                Password = BC.HashPassword(request.Password),
                ActivationToken = activationToken,
                ActivationTokenExpiresAt = expiresAt
            };

            _context.UserAccounts.Add(userAccount);
            await _context.SaveChangesAsync();

            // Create job to send activationToken to the account's e-mail

            return new OkResponse("Account created.");
        }

        public async Task<OkResponse> DeleteUserAccount(DeleteUserAccountRequest request)
        {
            var user = await _context.Users
                .Where(u => u.UserAccountID == request.ID)
                .Include(u => u.UserAccount)
                .SingleOrDefaultAsync();

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            _context.UserAccounts.Remove(user.UserAccount);

            await _context.SaveChangesAsync();

            return new OkResponse("Account deleted.");
        }

        public async Task<AuthenticationResponse> Authenticate(AuthenticateRequest request)
        {
            var userAccount = await _context.UserAccounts.AsNoTracking()
                .Where(a => a.Email.Equals(request.Email))
                .SingleOrDefaultAsync();

            if (userAccount == null || !BC.Verify(request.Password, userAccount.Password))
            {
                throw new Exception("Invalid e-mail or password.");
            }

            if (!userAccount.IsActive)
            {
                throw new Exception("Account is not active.");
            }

            if (!userAccount.IsAccountActivated)
            {
                throw new Exception("Account was not activated.");
            }

            var user = await _context.Users
                .Where(u => u.UserAccountID == userAccount.ID)
                .Include(u => u.RefreshTokens)
                .Include(u => u.Roles)
                .SingleOrDefaultAsync();

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            CreateUserActivityLog(user.ID, "User performed authentication", request.IpAddress, request.Agent, request.Origin);

            var token = GenerateJwtToken(user.ID, request.IpAddress, request.Agent, request.Origin);

            var refreshToken = GenerateRefreshToken(user.ID);
            user.RefreshTokens.Add(refreshToken);

            RemoveOldRefreshTokens(user);

            await _context.SaveChangesAsync();

            return new AuthenticationResponse
            {
                UserID = user.ID,
                Token = token,
                RefreshToken = refreshToken.Token,
                Roles = user.Roles.Select(c => c.Code).ToArray()
            };
        }

        public async Task<AuthenticationResponse> RefreshToken(RefreshTokenRequest request)
        {
            var (refreshToken, user) = await GetRefreshToken(request.RefreshToken);

            var newRefreshToken = GenerateRefreshToken(user.ID);
            refreshToken.RevokedAt = DateTime.UtcNow;
            user.RefreshTokens.Add(newRefreshToken);

            CreateUserActivityLog(user.ID, $"Replaced refresh token {refreshToken.Token} by {newRefreshToken.Token}", request.IpAddress, request.Agent, request.Origin);

            RemoveOldRefreshTokens(user);

            var token = GenerateJwtToken(user.ID, request.IpAddress, request.Agent, request.Origin);

            await _context.SaveChangesAsync();

            return new AuthenticationResponse
            {
                UserID = user.ID,
                Token = token,
                RefreshToken = newRefreshToken.Token,
                Roles = user.Roles.Select(c => c.Code).ToArray()
            };
        }

        public async Task<OkResponse> RevokeToken(RevokeTokenRequest request)
        {
            var (refreshToken, user) = await GetRefreshToken(request.RefreshToken);
            refreshToken.RevokedAt = DateTime.UtcNow;

            CreateUserActivityLog(user.ID, $"Revoked refresh token {refreshToken.Token}", request.IpAddress, request.Agent, request.Origin);

            await _context.SaveChangesAsync();

            return new OkResponse("Token revoked");
        }

        public async Task<OkResponse> ActivateAccount(ActivateAccountRequest request)
        {
            var userAccount = await _context.UserAccounts
                .Where(a => a.ActivationToken.Equals(request.ActivationToken))
                .SingleOrDefaultAsync();

            if (userAccount == null)
            {
                throw new Exception("Invalid token.");
            }

            if (userAccount.ActivationTokenExpiresAt < DateTime.UtcNow)
            {
                throw new Exception("Token has expired");
            }

            userAccount.ActivationToken = null;
            userAccount.ActivationTokenExpiresAt = null;
            userAccount.IsAccountActivated = true;

            var user = new User
            {
                Name = request.Name,
                UserAccountID = userAccount.ID
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            CreateUserActivityLog(user.ID, $"Account has been activated [Token: {request.ActivationToken}]", request.IpAddress, request.Agent, request.Origin);

            var userRole = await _context.Roles.AsNoTracking()
                .Where(r => r.Code.Equals("SystemUser"))
                .SingleOrDefaultAsync();

            if (userRole != null)
            {
                user.Roles.Add(userRole);
            }

            await _context.SaveChangesAsync();

            return new OkResponse("Account has been activated.");
        }

        public async Task<OkResponse> ResetPassword(ResetPasswordRequest request)
        {
            var user = await _context.Users
                .Where(u => u.UserAccount.ResetToken.Equals(request.ResetToken))
                .Include(u => u.UserAccount)
                .SingleOrDefaultAsync();

            if (user.UserAccount == null)
            {
                throw new Exception("Invalid token.");
            }

            if (user.UserAccount.ResetTokenExpiresAt < DateTime.UtcNow)
            {
                throw new Exception("Token has expired");
            }

            user.UserAccount.ResetToken = null;
            user.UserAccount.ResetTokenExpiresAt = null;
            user.UserAccount.Password = BC.HashPassword(request.Password);

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
                var (token, expiresAt) = await GenerateRandomToken((TokenType)request.Type);

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

                // Create job to send token to the account's e-mail
            }

            return new OkResponse("An e-mail has been sent to your e-mail address.");
        }

        private string GenerateJwtToken(int userID, string? IpAddress, string? agent, string? origin)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("userID", userID.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            CreateUserActivityLog(userID, $"Generated JWT valid from {token.ValidFrom} to {token.ValidTo} [{token.Issuer}]", IpAddress, agent, origin);

            return tokenHandler.WriteToken(token);
        }

        private RefreshToken GenerateRefreshToken(int userID)
        {
            var refreshToken = new RefreshToken
            {
                Token = RandomTokenString(),
                UserID = userID,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            return refreshToken;
        }

        private async Task<(RefreshToken, User)> GetRefreshToken(string token)
        {
            var user = await _context.Users
                .Where(u => u.RefreshTokens.Any(t => t.Token == token))
                .Include(u => u.RefreshTokens)
                .Include(u => u.Roles)
                .SingleOrDefaultAsync();

            if (user == null)
            {
                throw new Exception("Invalid token");
            }

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (!refreshToken.IsActive)
            {
                throw new Exception("Invalid token");
            }

            return (refreshToken, user);
        }

        private void RemoveOldRefreshTokens(User user)
        {
            user.RefreshTokens.RemoveAll(x => !x.IsActive);
        }

        private void CreateUserActivityLog(int userID, string content, string? IpAddress, string? agent, string? origin)
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

        private async Task<(string, DateTime)> GenerateRandomToken(TokenType type)
        {
            var users = await _context.UserAccounts.AsNoTracking().ToListAsync();
            var random = new Random();
            string randomToken;

            do
            {
                randomToken = (random.Next(00014423, 99949031)).ToString("00000000");
            }
            while ((type == TokenType.ActivationToken && users.Any(u => u.ActivationToken == randomToken)) || (type == TokenType.ResetToken && users.Any(u => u.ResetToken == randomToken)));

            return (randomToken, DateTime.UtcNow.AddHours(4));
        }

        private string RandomTokenString()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(40);
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        private enum TokenType
        {
            ActivationToken = 0,
            ResetToken = 1,
        }
    }
}
