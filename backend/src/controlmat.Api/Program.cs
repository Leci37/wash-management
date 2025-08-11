using Serilog;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .Enrich.WithEnvironmentUserName());

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

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
