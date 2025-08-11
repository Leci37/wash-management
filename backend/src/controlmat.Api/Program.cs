using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using Serilog.Enrichers;
using Controlmat.Application;
using Controlmat.Infrastructure;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Serilog Logging
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithThreadId()
    .Enrich.WithEnvironmentUserName()
    .WriteTo.Console()
    .WriteTo.File("Logs/sumisan-log-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Log.Warning("JWT Authentication failed: {Error}", context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var userName = context.Principal?.FindFirst("userName")?.Value;
                Log.Information("JWT Token validated for user: {UserName}", userName);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("WarehouseUser", policy =>
        policy.RequireRole("WarehouseUser", "Supervisor", "Administrator"));
    options.AddPolicy("Supervisor", policy =>
        policy.RequireRole("Supervisor", "Administrator"));
    options.AddPolicy("Administrator", policy =>
        policy.RequireRole("Administrator"));
});

// CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "SUMISAN API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token"
    });
    c.AddSecurityRequirement(new()
    {
        {
            new() { Reference = new() { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            new string[] { }
        }
    });
});

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Display registered routes on startup in a distinct color
var endpointSources = app.Services.GetRequiredService<IEnumerable<EndpointDataSource>>();
var routes = endpointSources
    .SelectMany(source => source.Endpoints)
    .OfType<RouteEndpoint>()
    .Select(endpoint => new
    {
        Pattern = endpoint.RoutePattern.RawText,
        Methods = endpoint.Metadata
            .OfType<HttpMethodMetadata>()
            .FirstOrDefault()?.HttpMethods ?? Array.Empty<string>()
    });

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("Registered endpoints:");
foreach (var route in routes)
{
    Console.WriteLine($"  [{string.Join(',', route.Methods)}] /{route.Pattern}");
}
Console.ResetColor();

app.Run();
