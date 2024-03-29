﻿using System.Net;
using System.Text.Json;

namespace Auth_Jwt.Infrastructure.Exceptions;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(
        RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(
        HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(
                context,
                ex);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        object errors = null;

        switch (exception)
        {
            case HttpException re:
                errors = re.Errors;
                context.Response.StatusCode = (int) re.Code;
                break;
            case Exception e:
                errors = string.IsNullOrWhiteSpace(e.Message) ? "Error" : e.Message;
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                break;
        }

        context.Response.ContentType = "application/json";

        var result = JsonSerializer.Serialize(
            new
            {
                errors
            });

        await context.Response.WriteAsync(result);
    }
}