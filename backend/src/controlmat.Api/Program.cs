using Serilog;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.IO;
using AutoMapper;
using MediatR;
using FluentValidation;
using Controlmat.Application;
using Controlmat.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Ensure the log directory exists
Directory.CreateDirectory("Logs");

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .WriteTo.File("Logs/controlmat-.log", rollingInterval: RollingInterval.Day)
    .Enrich.WithEnvironmentUserName());

// Dependency registrations
builder.Services.AddAutoMapper(typeof(Controlmat.Application.Common.Mappings.MappingProfile));
builder.Services.AddHttpContextAccessor();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Controlmat.Application.Common.Commands.WashCycle.StartWashCommand).Assembly);
});
builder.Services.AddValidatorsFromAssemblyContaining<Controlmat.Application.Common.Validators.NewWashDtoValidator>();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "controlmat.Api", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }, new string[] {}
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var config = builder.Configuration;
        options.Authority = config["Keycloak:Authority"];
        options.Audience = config["Keycloak:Audience"];
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RoleClaimType = "roles",
            NameClaimType = "preferred_username"
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
