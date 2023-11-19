using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Model;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly IList<string> _roles;

    public AuthorizeAttribute(params string[] roles)
    {
        _roles = roles ?? new string[] { };
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = (User)context.HttpContext.Items["User"];

        if (user == null)
        {
            throw new Exception("Unauthorized");
        }

        var unauthorized = user == null;
        var forbidden = _roles.Any() && !_roles.Any(s => user.Roles.Select(r => r.Code).ToArray().Contains(s));

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
