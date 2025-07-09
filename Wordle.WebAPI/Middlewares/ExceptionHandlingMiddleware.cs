using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using Wordle.Application.Common.Exceptions;

namespace Wordle.WebAPI.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Global hata yakalandı.");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ex switch
            {
                NotFoundException => (int)HttpStatusCode.NotFound,
                ValidationAppException => (int)HttpStatusCode.BadRequest,
                UnauthorizedAppException => (int)HttpStatusCode.Unauthorized,
                ForbiddenException => (int)HttpStatusCode.Forbidden,
                ConflictException => (int)HttpStatusCode.Conflict,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var response = new { error = ex.Message };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        
        if (context.Response.StatusCode == StatusCodes.Status401Unauthorized && !context.Response.HasStarted)
        {
            context.Response.ContentType = "application/json";
            var response = new { error = "Yetkisiz işlem" };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

    }
}
