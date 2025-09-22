using CasbinMinimalApi.Domain;
using CasbinMinimalApi.Endpoints.Authorization;
using Microsoft.AspNetCore.Authentication;

namespace CasbinMinimalApi.Endpoints;

public static class ApiExtensions
{
    public static void MapApiEndpoints(this WebApplication app)
    {
        app.MapIdentityApi<NeighborUser>();
        app.MapGet("/login/keycloak", (HttpContext ctx) =>
        {
            var props = new AuthenticationProperties { RedirectUri = "/" };
            return Results.Challenge(props, ["keycloak"]);
        });
        app.MapAuthorizationEndpoints();
        app.MapNeighborEndpoints();
        app.MapStuffEndpoints();
    }
}