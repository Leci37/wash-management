using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Controlmat.Domain.Interfaces;
using Controlmat.Infrastructure.Persistence;
using Controlmat.Infrastructure.Repositories;
using Controlmat.Infrastructure.Services;

namespace Controlmat.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<ControlmatDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Repositories
            services.AddScoped<IWashingRepository, WashingRepository>();
            services.AddScoped<IProtRepository, ProtRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMachineRepository, MachineRepository>();

            // Services
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            return services;
        }
    }
}
