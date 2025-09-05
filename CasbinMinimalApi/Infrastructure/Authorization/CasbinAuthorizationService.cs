using Casbin;
using CasbinMinimalApi.Application.Authorization;

namespace CasbinMinimalApi.Infrastructure.Authorization;

public class CasbinAuthorizationService(IEnforcer enforcer) : IAuthorizationService
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
}