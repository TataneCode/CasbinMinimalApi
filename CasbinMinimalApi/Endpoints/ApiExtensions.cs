using CasbinMinimalApi.Domain;
using CasbinMinimalApi.Endpoints.Authorization;

namespace CasbinMinimalApi.Endpoints;

public static class ApiExtensions
{
    public static void MapApiEndpoints(this WebApplication app)
    {
        app.MapIdentityApi<NeighborUser>();
        app.MapAuthorizationEndpoints();
        app.MapNeighborEndpoints();
        app.MapStuffEndpoints();
    }
}