using System.Text;
using System.Text.Json;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Messaging;

namespace ECommerce.Api.Middleware;

public sealed class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, IMessagePublisher publisher, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.EnableBuffering();
        var requestBody = await new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true).ReadToEndAsync();
        context.Request.Body.Position = 0;

        var originalBodyStream = context.Response.Body;
        await using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        try
        {
            var log = new RequestResponseLog
            {
                TraceId = context.TraceIdentifier,
                Method = context.Request.Method,
                Path = context.Request.Path,
                StatusCode = context.Response.StatusCode,
                RequestBody = requestBody,
                ResponseBody = responseText,
                RequestHeaders = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                ResponseHeaders = context.Response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())
            };
            
            _logger.LogInformation("Publishing log for {Method} {Path} with status {StatusCode}", 
                log.Method, log.Path, log.StatusCode);
            
            await _publisher.PublishAsync(log, context.RequestAborted);
            
            _logger.LogInformation("Successfully published log to RabbitMQ for {Method} {Path}", 
                log.Method, log.Path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed publishing request/response log");
        }
        finally
        {
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }
}


