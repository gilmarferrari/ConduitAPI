﻿using Microsoft.AspNetCore.Mvc;
using Model;

namespace ConduitAPI.Controllers
{
    [Controller]
    public abstract class BaseController : ControllerBase
    {
        public User LoggedUser => (User)HttpContext.Items["User"];

        public string IPAddress => Request.Headers
            .ContainsKey("X-Forwarded-For") ? Request.Headers["X-Forwarded-For"] : HttpContext.Connection.RemoteIpAddress
            .MapToIPv4()
            .ToString();

        public string Agent => Request.Headers.UserAgent;

        public string Origin => Request.Headers.Origin;
    }
}
