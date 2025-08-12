using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using controlmat.Domain.Interfaces;
using controlmat.Infrastructure.Persistence;
using controlmat.Infrastructure.Repositories;

namespace controlmat.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ✅ Database Context
        services.AddDbContext<SumisanDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.CommandTimeout(30);
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null);
            });

            // Enable sensitive data logging in development
            if (configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        // ✅ Repository Registration (Scoped for EF Core)
        services.AddScoped<IWashingRepository, WashingRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IMachineRepository, MachineRepository>();
        services.AddScoped<IProtRepository, ProtRepository>();
        services.AddScoped<IPhotoRepository, PhotoRepository>();
        services.AddScoped<IParameterRepository, ParameterRepository>();

        return services;
    }
}

