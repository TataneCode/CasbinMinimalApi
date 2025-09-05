using CasbinMinimalApi.Application.Authorization;

namespace CasbinMinimalApi.Endpoints.Authorization;

public static class AuthorizationEndpoints
{
  public static void MapAuthorizationEndpoints(this IEndpointRouteBuilder app)
  {
    var endpoints = app.MapGroup("/casbin").WithTags("Authorization").RequireAuthorization();

    endpoints.MapPermissionCheck();
    endpoints.MapUserRoleEndpoints();
    endpoints.MapRoleUserEndpoints();
    endpoints.MapRolePermissionEndpoints();
  }

  private static void MapPermissionCheck(this RouteGroupBuilder group)
  {
    group.MapGet("/permission/check", CheckPermissionAsync)
      .WithName("CheckPermission")
      .WithSummary("Check if a user has a permission (user, resource, action)")
      .Produces(StatusCodes.Status200OK);
  }

  private static void MapUserRoleEndpoints(this RouteGroupBuilder group)
  {
    group.MapGet("/users/{user}/roles", GetRolesForUser)
      .WithName("GetRolesForUser")
      .WithSummary("List roles assigned to a user")
      .Produces<string[]>(StatusCodes.Status200OK);

    group.MapPost("/users/{user}/roles/{role}", AddRoleForUserAsync)
      .WithName("AddRoleForUser")
      .WithSummary("Assign a role to a user")
      .Produces(StatusCodes.Status200OK);

    group.MapDelete("/users/{user}/roles/{role}", RemoveRoleForUserAsync)
      .WithName("RemoveRoleForUser")
      .WithSummary("Remove a role from a user")
      .Produces(StatusCodes.Status200OK);
  }

  private static void MapRoleUserEndpoints(this RouteGroupBuilder group)
  {
    group.MapGet("/roles/{role}/users", GetUsersForRole)
      .WithName("GetUsersForRole")
      .WithSummary("List users assigned to a role")
      .Produces<string[]>(StatusCodes.Status200OK);
  }

  private static void MapRolePermissionEndpoints(this RouteGroupBuilder group)
  {
    group.MapPost("/roles/{role}/permissions", AddPermissionForRoleAsync)
      .WithName("AddPermissionForRole")
      .WithSummary("Add a permission (resource, action) to a role")
      .Produces(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status400BadRequest);

    group.MapDelete("/roles/{role}/permissions", RemovePermissionForRoleAsync)
      .WithName("RemovePermissionForRole")
      .WithSummary("Remove a permission (resource, action) from a role")
      .Produces(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status400BadRequest);
  }

  // Handlers
  private static async Task<IResult> CheckPermissionAsync([AsParameters] PermissionQuery query, IAuthorizationService service)
  {
    var allowed = await service.HasPermissionAsync(query.User, query.Resource, query.Action);
    return Results.Ok(new { allowed });
  }

  private static IResult GetRolesForUser(string user, IAuthorizationService service)
  {
    var roles = service.GetRolesForUserAsync(user);
    return Results.Ok(roles);
  }

  private static async Task<IResult> AddRoleForUserAsync(string user, string role, IAuthorizationService service)
  {
    var added = await service.AddRoleForUserAsync(user, role);
    return Results.Ok(new { added });
  }

  private static async Task<IResult> RemoveRoleForUserAsync(string user, string role, IAuthorizationService service)
  {
    var removed = await service.RemoveRoleForUserAsync(user, role);
    return Results.Ok(new { removed });
  }

  private static IResult GetUsersForRole(string role, IAuthorizationService service)
  {
    var users = service.GetUsersForRoleAsync(role);
    return Results.Ok(users);
  }

  private static async Task<IResult> AddPermissionForRoleAsync(string role, AddPermissionRequest body, IAuthorizationService service)
  {
    if (string.IsNullOrWhiteSpace(body.Resource) || string.IsNullOrWhiteSpace(body.Action))
    {
      return Results.BadRequest("Resource and Action are required");
    }
    var added = await service.AddPermissionForRoleAsync(role, body.Resource, body.Action);
    return Results.Ok(new { added });
  }

  private static async Task<IResult> RemovePermissionForRoleAsync(string role, [AsParameters] PermissionBodyOrQuery query, IAuthorizationService service)
  {
    if (string.IsNullOrWhiteSpace(query.Resource) || string.IsNullOrWhiteSpace(query.Action))
    {
      return Results.BadRequest("Resource and Action are required");
    }
    var removed = await service.RemovePermissionForRoleAsync(role, query.Resource, query.Action);
    return Results.Ok(new { removed });
  }

  // Query record for /permission/check
  public record PermissionQuery(string User, string Resource, string Action);

  // Body for adding permission
  public record AddPermissionRequest(string Resource, string Action);

  // Allow passing resource/action either via query in delete
  public record PermissionBodyOrQuery(string Resource, string Action);
}
