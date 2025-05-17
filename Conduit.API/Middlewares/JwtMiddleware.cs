using Conduit.Infrastructure.Data;
using Conduit.Application.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Conduit.API.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ConduitContext dbContext)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                await AttachUserToContext(context, dbContext, token);
            }

            await _next(context);
        }

        private async Task AttachUserToContext(HttpContext context, ConduitContext dbContext, string token)
        {
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
                var userID = Guid.Parse(jwtToken.Claims.First(x => x.Type == "userID").Value);

                context.Items["User"] = await dbContext.Users.AsNoTracking()
                    .Where(u => u.ID == userID)
                    .Include(u => u.Roles)
                    .Include(u => u.LongLivedTokens)
                    .Include(u => u.UserAccount)
                    .SingleOrDefaultAsync();
            }
            catch (Exception) { }
        }
    }
}