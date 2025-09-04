using CasbinMinimalApi.Domain;
using CasbinMinimalApi.Endpoints.WeatherForecast;

namespace CasbinMinimalApi.Endpoints;

public static class ApiExtensions
{
    public static void MapApiEndpoints(this WebApplication app)
    {
        app.MapWeatherApiEndpoints();
        app.MapIdentityApi<NeighborUser>();
    }
}