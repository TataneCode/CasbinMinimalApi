using System.Net;
using System.Text.Json;
using CasbinMinimalApi.Application.Authorization;

namespace CasbinMinimalApi.Infrastructure.Authorization;

public sealed record CasbinRequirement(string Resource, string Action);

public static class CasbinRouteBuilderExtensions
{
  public static RouteHandlerBuilder WithCasbin(this RouteHandlerBuilder builder, string resource, string action)
      => builder.WithMetadata(new CasbinRequirement(resource, action));
}

public class CasbinAuthorizationMiddleware(RequestDelegate next)
{
  public async Task InvokeAsync(HttpContext context, IAuthorizationService auth, ILogger<CasbinAuthorizationMiddleware> logger)
  {
    var endpoint = context.GetEndpoint();
    var requirement = endpoint?.Metadata.GetMetadata<CasbinRequirement>();
    if (requirement is null)
    {
      await next(context);
      return;
    }

    var allowed = await auth.HasPermissionAsync(requirement.Resource, requirement.Action);
    if (!allowed)
    {
      await WriteJsonAsync(context, HttpStatusCode.Forbidden, new
      {
        error = "Ressource access is forbidden",
        message = $"Access denied for action \'{requirement.Action}\' on resource \'{requirement.Resource}\'."
      });
      return;
    }

    await next(context);
  }

  private static async Task WriteJsonAsync(HttpContext ctx, HttpStatusCode status, object payload)
  {
    if (ctx.Response.HasStarted) return;
    ctx.Response.StatusCode = (int)status;
    ctx.Response.ContentType = "application/json; charset=utf-8";
    await JsonSerializer.SerializeAsync(ctx.Response.Body, payload, new JsonSerializerOptions
    {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    });
  }
}
