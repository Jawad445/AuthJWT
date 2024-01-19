using Auth_Jwt.Infrastructure.Exceptions;
using Auth_Jwt.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Auth_Jwt;

public static class Extensions
{
    public static void AddJwt(this IServiceCollection services)
    {
        services.AddOptions();

        var signingCredentials = services.BuildServiceProvider().GetService<IOptions<JwtSettings>>();

        services.Configure<JwtIssuerOptions>( options =>
            {
                options.Issuer = signingCredentials.Value.Issuer;
                options.Audience = signingCredentials.Value.Audience;
                options.SigningCredentials = signingCredentials.Value.SigningCredentials;
            });

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingCredentials.Value.SigningCredentials.Key,
            ValidateIssuer = true,
            ValidIssuer = signingCredentials.Value.Issuer,
            ValidateAudience = true,
            ValidAudience = signingCredentials.Value.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer( options =>
            {
                options.TokenValidationParameters = tokenValidationParameters;
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = (
                        context) =>
                    {
                        var token = context.HttpContext.Request.Headers["Authorization"];
                        if (token.Count > 0 && token[0].StartsWith(
                                "Token ",
                                StringComparison.OrdinalIgnoreCase))
                        {
                            context.Token = token[0].Substring("Token ".Length).Trim();
                        }

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add(
                                "Token-Expired",
                                "true");
                        }

                        return Task.CompletedTask;
                    }
                };
            });
    }

    public static IApplicationBuilder UseMiddlewares(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }

}
