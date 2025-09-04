using Casbin;
using Casbin.Persist;
using Casbin.Persist.Adapter.File;

namespace CasbinMinimalApi.Casbin;

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

public class CasbinAuthorizationService(IEnforcer enforcer) : IAuthorizationService
{
  private readonly IEnforcer _enforcer = enforcer;

  public async Task<bool> HasPermissionAsync(string user, string resource, string action)
  {
    return await _enforcer.EnforceAsync(user, resource, action);
  }

  public async Task<bool> AddRoleForUserAsync(string user, string role)
  {
    return await _enforcer.AddRoleForUserAsync(user, role);
  }

  public async Task<bool> RemoveRoleForUserAsync(string user, string role)
  {
    return await _enforcer.DeleteRoleForUserAsync(user, role);
  }

  public string[] GetRolesForUserAsync(string user)
  {
    return [.. _enforcer.GetRolesForUser(user)];
  }

  public string[] GetUsersForRoleAsync(string role)
  {
    return [.. _enforcer.GetUsersForRole(role)];
  }

  public async Task<bool> AddPermissionForRoleAsync(string role, string resource, string action)
  {
    return await _enforcer.AddPermissionForUserAsync(role, resource, action);
  }

  public async Task<bool> RemovePermissionForRoleAsync(string role, string resource, string action)
  {
    return await _enforcer.DeletePermissionForUserAsync(role, resource, action);
  }
}

public interface IInitializationService
{
  Task LoadPoliciesAsync();
}

public class CasbinInitializationService(
  IAdapter adapter,
  IEnforcer enforcer,
  IWebHostEnvironment environment,
  ILogger<CasbinInitializationService> logger) : IInitializationService
{

  private readonly IAdapter _adapter = adapter;
  private readonly IEnforcer _enforcer = enforcer;
  private readonly IWebHostEnvironment _environment = environment;
  private readonly ILogger<CasbinInitializationService> _logger = logger;

  public async Task LoadPoliciesAsync()
  {

    try
    {

      var policyPath = Path.Combine(_environment.ContentRootPath, "Casbin", "rbac_policy.csv");

      if (!File.Exists(policyPath))
      {
        _logger.LogError("Casbin configuration files not found");
        return;
      }

      // Load policies from file adapter
      var fileAdapter = new FileAdapter(policyPath);
      _enforcer.SetAdapter(fileAdapter);
      await _enforcer.LoadPolicyAsync();

      // Set EF Adapter ans persist to database
      _enforcer.SetAdapter(_adapter);
      await _enforcer.SavePolicyAsync();

      _logger.LogInformation("Casbin policies loaded from files and saved to database");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to initialize Casbin policies from files");
    }
  }
}