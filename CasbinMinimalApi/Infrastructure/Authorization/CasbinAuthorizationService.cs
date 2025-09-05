using System.Security.Claims;
using Casbin;
using CasbinMinimalApi.Application.Authorization;

namespace CasbinMinimalApi.Infrastructure.Authorization;

public class CasbinAuthorizationService(
    IEnforcer enforcer,
    IHttpContextAccessor contextAccessor) : IAuthorizationService
{
    public async Task<bool> HasPermissionAsync(string user, string resource, string action)
    {
        return await enforcer.EnforceAsync(user, resource, action);
    }

    public async Task<bool> AddRoleForUserAsync(string user, string role)
    {
        return await enforcer.AddRoleForUserAsync(user, role);
    }

    public async Task<bool> RemoveRoleForUserAsync(string user, string role)
    {
        return await enforcer.DeleteRoleForUserAsync(user, role);
    }

    public string[] GetRolesForUserAsync(string user)
    {
        return [.. enforcer.GetRolesForUser(user)];
    }

    public string[] GetUsersForRoleAsync(string role)
    {
        return [.. enforcer.GetUsersForRole(role)];
    }

    public async Task<bool> AddPermissionForRoleAsync(string role, string resource, string action)
    {
        return await enforcer.AddPermissionForUserAsync(role, resource, action);
    }

    public async Task<bool> RemovePermissionForRoleAsync(string role, string resource, string action)
    {
        return await enforcer.DeletePermissionForUserAsync(role, resource, action);
    }

    public bool HasCurrentUserRole(string role)
    {
        var email = GetCurrentUserEmail();
        if (string.IsNullOrWhiteSpace(email)) return false;
        var roles = enforcer.GetRolesForUser(email);

        return roles.Contains(role);
    }

    public bool HasCurrentUserAnyRole(params string[] roles)
    {
        var email = GetCurrentUserEmail();
        if (string.IsNullOrWhiteSpace(email)) return false;
        var userRoles = enforcer.GetRolesForUser(email);
        return userRoles.Intersect(roles, StringComparer.OrdinalIgnoreCase).Any();
    }

    private string? GetCurrentUserEmail()
        => contextAccessor.HttpContext?
            .User?
            .FindFirstValue(ClaimTypes.Email);
}