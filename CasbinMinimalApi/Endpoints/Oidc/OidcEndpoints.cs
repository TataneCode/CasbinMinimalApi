using CasbinMinimalApi.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace CasbinMinimalApi.Endpoints.Oidc;

public static class OidcEndpoints
{
    public static IEndpointRouteBuilder MapOidcEndpoints(this IEndpointRouteBuilder builder)
    {
        if (Environment.GetEnvironmentVariable(ConfigurationKey.OpenIdEnabled) == OpenIdStatus.Disabled)
            return builder;
        
        var group = builder.MapGroup("/oidc").WithTags("Oidc connection");
        
        group.MapGet("/challenge", (HttpContext ctx) =>
        {
            var props = new AuthenticationProperties { RedirectUri = "/oidc/signedin" };
            return Results.Challenge(props, [OpenIdConnectDefaults.AuthenticationScheme]);
        });
        group.MapGet("/signedin", () => Results.Ok(new { Message = "You have been signed in" }));
        
        return group;
    }
}