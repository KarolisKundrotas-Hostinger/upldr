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
        return "Unauthorized";
    }
    var filePath = Path.Combine(configUploadPath, usedFileName);
    await using var fileStream = File.Create(filePath);
    await context.Request.Body.CopyToAsync(fileStream);
    logger.LogInformation($"Uploaded file to '{filePath}'");
    return "OK";
});

app.Run();