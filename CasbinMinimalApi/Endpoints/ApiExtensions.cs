using CasbinMinimalApi.Domain;
using CasbinMinimalApi.Endpoints.Authorization;
using Microsoft.AspNetCore.Authentication;

namespace CasbinMinimalApi.Endpoints;

public static class ApiExtensions
{
    public static void MapApiEndpoints(this WebApplication app)
    {
        app.MapIdentityApi<NeighborUser>();
        app.MapGet("/login-oidc", (HttpContext ctx) =>
        {
            var props = new AuthenticationProperties { RedirectUri = "/signin-oidc" };
            return Results.Challenge(props, ["zitadel"]);
        });
        app.MapGet("signin-oidc", () => Results.Ok(new { Message = "You have been signed in" })); ;
        app.MapAuthorizationEndpoints();
        app.MapNeighborEndpoints();
        app.MapStuffEndpoints();
    }
}