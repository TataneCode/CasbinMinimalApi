using CasbinMinimalApi.Domain;
using CasbinMinimalApi.Endpoints.Authorization;
using CasbinMinimalApi.Endpoints.Oidc;

namespace CasbinMinimalApi.Endpoints;

public static class ApiExtensions
{
    public static void MapApiEndpoints(this WebApplication app)
    {
        app.MapIdentityApi<NeighborUser>();
        app.MapOidcEndpoints();
        app.MapAuthorizationEndpoints();
        app.MapNeighborEndpoints();
        app.MapStuffEndpoints();
    }
}