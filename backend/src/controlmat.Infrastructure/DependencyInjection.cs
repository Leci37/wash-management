using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using controlmat.Domain.Interfaces;
using controlmat.Infrastructure.Persistence;
using controlmat.Infrastructure.Repositories;

namespace controlmat.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ControlmatDbContext>(options =>
            options.UseInMemoryDatabase("ControlmatDb"));

        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }
}

