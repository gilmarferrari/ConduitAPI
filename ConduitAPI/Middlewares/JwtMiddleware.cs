using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Model.DataModels;
using Services.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ConduitAPI.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;

        public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
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
                var userID = int.Parse(jwtToken.Claims.First(x => x.Type == "userID").Value);

                context.Items["User"] = await dbContext.Users.AsNoTracking()
                    .Include(u => u.Roles)
                    .Include(u => u.RefreshTokens)
                    .Include(u => u.UserAccount)
                    .Where(u => u.ID == userID)
                    .SingleOrDefaultAsync();
            }
            catch
            { }
        }
    }
}