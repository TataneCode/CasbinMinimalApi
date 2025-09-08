namespace CasbinMinimalApi.Application.Authorization;

public interface IRoleService
{
    Task LoadPoliciesAsync();

    Task<bool> HasAnyPoliciesAsync();
}