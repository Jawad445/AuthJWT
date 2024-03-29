﻿using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Auth_Jwt.Infrastructure.Security;

public interface ICurrentUser
{
    string EmailId { get; }
}

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string EmailId => _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
}