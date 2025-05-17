using Conduit.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Conduit.API.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly IList<RoleList> _roles;

        public AuthorizeAttribute(params RoleList[] roles)
        {
            _roles = roles ?? Array.Empty<RoleList>();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = (User)context.HttpContext.Items["User"];

            if (user == null)
            {
                throw new Exception("Unauthorized");
            }

            var unauthorized = user == null;
            var forbidden = _roles.Any() && !_roles.Any(r => user.Roles.Select(x => (RoleList)x.Role).ToArray().Contains(r));

            if (unauthorized)
            {
                context.Result = new JsonResult(new { message = "Unauthorized" })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }
            else if (forbidden)
            {
                context.Result = new JsonResult(new { message = "Forbidden" })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            };
        }
    }
}
