using CasbinMinimalApi.Constants;
using Microsoft.AspNetCore.Authentication;

namespace CasbinMinimalApi.Endpoints.Oidc;

public static class OidcEndpoints
{
    public static IEndpointRouteBuilder MapOidcEndpoints(this IEndpointRouteBuilder builder)
    {
        if (Environment.GetEnvironmentVariable(ConfigurationKey.OpenIdEnabled) == OpenIdStatus.Disabled)
            return builder;
        
        var group = builder.MapGroup("/oidc").WithTags("Oidc connection");
        
        group.MapGet("/login", (HttpContext ctx) =>
        {
            var props = new AuthenticationProperties { RedirectUri = "/signedin" };
            return Results.Challenge(props, ["zitadel"]);
        });
        group.MapGet("/signedin", () => Results.Ok(new { Message = "You have been signed in" }));
        
        return group;
    }
}