using MongoDB.Driver;
using System.Text.Json.Serialization;
using TaskManagement.Api.Middleware;
using TaskManagement.Api.Repositories;
using TaskManagement.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// ─── MongoDB ─────────────────────────────────────────────────────────────────
// Connection string resolution order:
//  1. MONGODB_URI environment variable  (Render.com secret env var)
//  2. MongoDbSettings:ConnectionString in appsettings.json
var mongoConnectionString =
    Environment.GetEnvironmentVariable("MONGODB_URI")
    ?? builder.Configuration["MongoDbSettings:ConnectionString"]
    ?? throw new InvalidOperationException(
        "MongoDB connection string is not configured. " +
        "Set the MONGODB_URI environment variable or MongoDbSettings:ConnectionString in appsettings.json.");

var mongoDatabaseName =
    builder.Configuration["MongoDbSettings:DatabaseName"] ?? "TaskManagementDb";

builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConnectionString));
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IMongoClient>().GetDatabase(mongoDatabaseName));

// ─── Application services ─────────────────────────────────────────────────────
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();

// ─── Controllers ─────────────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Accept and return status as strings: "Pending", "InProgress", "Completed"
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// ─── CORS ─────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactFrontend", policy =>
    {
        // In production set the exact origin e.g. https://your-app.vercel.app
        // via the ALLOWED_ORIGINS environment variable (comma-separated).
        var rawOrigins = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS");
        if (!string.IsNullOrWhiteSpace(rawOrigins))
        {
            var origins = rawOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            policy.WithOrigins(origins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            // Development: allow all origins
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});

// ─── Swagger ─────────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Task Management API",
        Version = "v1",
        Description = "A production-ready Task Management REST API built with ASP.NET Core 8 and MongoDB."
    });
});

// ─── Render.com port binding ─────────────────────────────────────────────────
// Render sets PORT environment variable; ASP.NET Core should listen on it.
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// ─── Build ─────────────────────────────────────────────────────────────────
var app = builder.Build();

// ─── Middleware pipeline ──────────────────────────────────────────────────────
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Management API v1");
    c.RoutePrefix = string.Empty; // Serve Swagger at root
});

app.UseCors("AllowReactFrontend");

app.UseAuthorization();
app.MapControllers();

app.Run();
