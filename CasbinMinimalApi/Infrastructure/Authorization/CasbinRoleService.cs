using Casbin;
using Casbin.Persist;
using Casbin.Persist.Adapter.File;
using CasbinMinimalApi.Application.Authorization;

namespace CasbinMinimalApi.Infrastructure.Authorization;

public class CasbinRoleService(
    IAdapter adapter,
    IEnforcer enforcer,
    IWebHostEnvironment environment,
    ILogger<CasbinRoleService> logger) : IRoleService
{
    public async Task LoadPoliciesAsync()
    {
        try
        {
            var policyPath = Path.Combine(environment.ContentRootPath, "Casbin", "rbac_policy.csv");

            if (!File.Exists(policyPath))
            {
                logger.LogError("Casbin configuration files not found");
                return;
            }

            // Load policies from file adapter
            var fileAdapter = new FileAdapter(policyPath);
            enforcer.SetAdapter(fileAdapter);
            await enforcer.LoadPolicyAsync();

            // Set EF Adapter ans persist to database
            enforcer.SetAdapter(adapter);
            await enforcer.SavePolicyAsync();

            logger.LogInformation("Casbin policies loaded from files and saved to database");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize Casbin policies from files");
        }
    }

    public async Task<bool> HasAnyPoliciesAsync()
    {
        try
        {
            enforcer.SetAdapter(adapter);
            await enforcer.LoadPolicyAsync();

            return enforcer.GetPolicy().Any() || enforcer.GetGroupingPolicy().Any();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to check existing Casbin policies");
            return false;
        }
    }
}