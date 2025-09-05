using CasbinMinimalApi.Application.Repositories;
using CasbinMinimalApi.Domain;

namespace CasbinMinimalApi.Startup;

public static class ServiceExtensions
{
    public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
    {
        // Built-in Services
        services.AddHttpContextAccessor();

        // Repositories
        services.AddScoped<INeighborRepository, NeighborRepository>();
        services.AddScoped<IStuffRepository, StuffRepository>();

        return services;
    }
}