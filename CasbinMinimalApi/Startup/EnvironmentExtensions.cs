using CasbinMinimalApi.Constants;
using Microsoft.EntityFrameworkCore;

namespace CasbinMinimanApi.Startup;

public static class EnvironmentExtensions
{
    public static void AnalyseConfiguration(this ConfigurationManager configuration)
    {
        var openIdClientId = configuration.GetValue<string>(ConfigurationKey.OpenIdClientId);
        var openIdClientSecret = configuration.GetValue<string>(ConfigurationKey.OpenIdClientSecret);
        var openIdAuthority = configuration.GetValue<string>(ConfigurationKey.OpenIdAuthority);

        var hasOpenId = !(string.IsNullOrEmpty(openIdClientId)
                          || string.IsNullOrEmpty(openIdClientSecret)
                          || string.IsNullOrEmpty(openIdAuthority));
        
        Environment.SetEnvironmentVariable(
            ConfigurationKey.OpenIdEnabled,
            hasOpenId ? OpenIdStatus.Enabled : OpenIdStatus.Disabled);
    }
}