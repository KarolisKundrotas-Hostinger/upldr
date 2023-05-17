using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/upload", async (
    HttpContext context, [FromServices] IConfiguration config, [FromServices] ILogger<Program> logger, 
    [FromHeader(Name = "X-File-Name")] string? fileName, [FromHeader(Name = "X-Key")] string? key) =>
{
    var configKey = config["Key"] ?? default;
    var configUploadPath = config["UploadPath"] ?? Path.GetTempPath();
    var usedFileName = fileName ?? "uploaded.bin";
    if (key != configKey)
    {
        context.Response.StatusCode = 401;
        logger.LogWarning($"Unauthorized access from \"{context.Connection.RemoteIpAddress}:{context.Connection.RemotePort}\"");
        return "Unauthorized";
    }
    if (context.Features.Get<IHttpMaxRequestBodySizeFeature>() is { } maxRequestBodySizeFeature)
    {
        maxRequestBodySizeFeature.MaxRequestBodySize = null;
    }
    var directory = Path.GetFullPath(configUploadPath);
    var filePath = Path.Combine(configUploadPath, usedFileName);
    var effectivePath = Path.GetFullPath(filePath);
    if (!effectivePath.StartsWith(directory))
    {
        context.Response.StatusCode = 403;
        logger.LogWarning($"Forbidden access, due to path traversal, from \"{context.Connection.RemoteIpAddress}:{context.Connection.RemotePort}\" to \"{filePath}\"");
        return "Forbidden";
    }
    await using var fileStream = File.Create(filePath);
    await context.Request.Body.CopyToAsync(fileStream);
    logger.LogInformation($"Uploaded file from \"{context.Connection.RemoteIpAddress}:{context.Connection.RemotePort}\" to \"{filePath}\"");
    return "OK";
});

app.MapGet("/", () => "OK");

app.Run();