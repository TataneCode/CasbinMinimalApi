using CasbinMinimalApi.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http.HttpResults;

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
        group.MapGet("/signedin", GetSignedInInformation).RequireAuthorization();

        return group;
    }

    private static Ok<UserInfo> GetSignedInInformation(IHttpContextAccessor contextAccessor)
    {
        var claims = contextAccessor.HttpContext?.User.Claims
            .Select(c => new UserClaim(c.Type, c.Value));
        var result = new UserInfo("You are signed in.", contextAccessor.HttpContext?.User.Identity?.Name, claims);

        return TypedResults.Ok(result);
    }
    
    private record UserInfo(string Message, string? Name, IEnumerable<UserClaim>? Claims);

    private record UserClaim(string Type, string Value);
}
