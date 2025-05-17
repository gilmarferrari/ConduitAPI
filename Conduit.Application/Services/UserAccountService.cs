using AutoMapper;
using Conduit.Application.Commands.Requests;
using Conduit.Application.Helpers;
using Conduit.Application.Queries.Requests;
using Conduit.Domain.Entities;
using Conduit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Conduit.Application.Commands.Responses;
using Conduit.Application.Queries.Responses;
using BC = BCrypt.Net.BCrypt;

namespace Conduit.Application.Services
{
    public interface IUserAccountService
    {
        Task<UserAccountResponse> GetUserAccount(GetUserAccountRequest request);
        Task<OkResponse> CreateUserAccount(CreateUserAccountRequest request);
        Task<OkResponse> DeleteUserAccount(DeleteUserAccountRequest request);
        Task<OkResponse> ActivateAccount(ActivateAccountRequest request);
    }

    public class UserAccountService : IUserAccountService
    {
        private readonly ConduitContext _context;
        private readonly IMapper _mapper;

        public UserAccountService(ConduitContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserAccountResponse> GetUserAccount(GetUserAccountRequest request)
        {
            var userAccount = await _context.UserAccounts.AsNoTracking()
                .Where(a => a.Email.Equals(request.Email))
                .SingleOrDefaultAsync();

            if (userAccount == null)
            {
                throw new Exception("Account not found.");
            }

            return _mapper.Map<UserAccountResponse>(userAccount);
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

            var (activationToken, expiresAt) = Extensions.GenerateRandomCode();

            var userAccount = new UserAccount
            {
                Email = request.Email,
                PasswordHash = BC.HashPassword(request.Password),
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
            var userAccount = await _context.UserAccounts
                .Where(a => a.Email.Equals(request.Email))
                .SingleOrDefaultAsync();

            if (userAccount == null)
            {
                throw new Exception("Account not found.");
            }

            _context.UserAccounts.Remove(userAccount);

            await _context.SaveChangesAsync();

            return new OkResponse("Account deleted.");
        }

        public async Task<OkResponse> ActivateAccount(ActivateAccountRequest request)
        {
            var userAccount = await _context.UserAccounts
                .Where(a => a.Email.Equals(request.Email))
                .SingleOrDefaultAsync();

            if (userAccount == null || userAccount.ActivationToken != request.ActivationToken)
            {
                throw new Exception("Invalid token.");
            }

            if (userAccount.ActivationTokenExpiresAt < DateTime.UtcNow)
            {
                throw new Exception("Token has expired");
            }

            userAccount.ActivationToken = null;
            userAccount.ActivationTokenExpiresAt = null;
            userAccount.ActivationDate = DateTime.UtcNow;

            var user = new User
            {
                Name = request.Name,
                UserAccountID = userAccount.ID
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            CreateUserActivityLog(user.ID, $"Account has been activated [Token: {request.ActivationToken}]", request.IpAddress, request.Agent, request.Origin);

            user.Roles.Add(new UserRole(user.ID, RoleList.SystemUser));

            await _context.SaveChangesAsync();

            return new OkResponse("Account has been activated.");
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
                UserID = userID,
            };

            _context.UserActivityLogs.Add(userActivityLog);
        }
    }
}
