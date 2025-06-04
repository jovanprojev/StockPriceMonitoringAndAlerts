using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockPriceMonitoringAndAlerts.BackgroundServices;
using StockPriceMonitoringAndAlerts.Converters;
using StockPriceMonitoringAndAlerts.Data;
using StockPriceMonitoringAndAlerts.Hubs;
using StockPriceMonitoringAndAlerts.Models;
using StockPriceMonitoringAndAlerts.Repositories;
using StockPriceMonitoringAndAlerts.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddScoped<IAlertRuleRepository, AlertRuleRepository>();
builder.Services.AddScoped<IAlertRuleService, AlertRuleService>();
builder.Services.AddScoped<IAlertRuleEvaluator, AlertRuleEvaluator>();
builder.Services.AddScoped<IStockSnapshotService, StockSnapshotService>();
builder.Services.AddScoped<IStockApiClient, StockApiClient>();
builder.Services.AddScoped<IStockPriceCacheService, StockPriceCacheService>();

builder.Services.AddSingleton<INotificationService, SignalRNotificationService>();

builder.Services.AddHttpClient<IStockApiClient, StockApiClient>();
builder.Services.AddSingleton<IStockPriceCacheService, StockPriceCacheService>();
builder.Services.AddHostedService<StockPollingService>();
builder.Services.AddMemoryCache();

// Add connection to the database
if (builder.Environment.EnvironmentName != "IntegrationTests")
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

// Configure JSON serialization to use custom DateTime format
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateTimeConverter_ddMMyyyy_HHmmss());
    });

// Configure JSON serialization to use enum as string
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Configure API behavior to return detailed validation errors
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );

        var result = new
        {
            message = "Validation failed",
            errors,
            allowedValues = new
            {
                StockSymbol = Enum.GetNames(typeof(StockSymbol)),
                Direction = Enum.GetNames(typeof(Direction))
            }
        };

        return new BadRequestObjectResult(result);
    };
});

// Configure CORS to allow any origin, header, and method
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:5230", "http://127.0.0.1:5230")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

var app = builder.Build();

app.UseCors();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.MapHub<NotificationHub>("/notificationHub");


// Apply any pending EF Core migrations at application startup.
// This is especially important when running in Docker containers,
// where the database may not be initialized yet.
// In local development (e.g., via Visual Studio), migrations can be run manually,
// but this ensures automatic setup when deploying or containerizing the app.
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Database migration failed at startup.");
    }
}

app.Run();

public partial class Program { }