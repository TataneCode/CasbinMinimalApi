namespace CasbinMinimalApi.Application.Authorization;


public interface IAuthorizationService
{
    Task<bool> HasPermissionAsync(string user, string resource, string action);

    Task<bool> AddRoleForUserAsync(string user, string role);

    Task<bool> RemoveRoleForUserAsync(string user, string role);

    string[] GetRolesForUserAsync(string user);

    string[] GetUsersForRoleAsync(string role);

    Task<bool> AddPermissionForRoleAsync(string role, string resource, string action);

    Task<bool> RemovePermissionForRoleAsync(string role, string resource, string action);

}