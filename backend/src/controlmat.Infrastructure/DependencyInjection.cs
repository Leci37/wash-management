using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using controlmat.Infrastructure.Persistence;

namespace controlmat.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ControlmatDbContext>(options =>
            options.UseInMemoryDatabase("ControlmatDb"));
        return services;
    }
}

