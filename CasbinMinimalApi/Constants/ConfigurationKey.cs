namespace CasbinMinimalApi.Constants;

internal static class ConfigurationKey
{
    internal const string ConnectionString = "PG_CONNECTION_STRING";
    internal const string DatabaseUser = "PGUSER";
    internal const string DatabasePassword = "PGPASSWORD";
    internal const string Environment = "ASPNETCORE_ENVIRONMENT";
    internal const string Urls = "ASPNETCORE_URLS";

    internal const string OpenIdClientId = "OPENID_CLIENT";
    internal const string OpenIdClientSecret = "OPENID_SECRET";
    internal const string OpenIdAuthority = "OPENID_AUTHORITY";
    internal const string DisabledTlsValidation = "DISABLE_TLS_VALIDATION";
    
    internal const string OpenIdEnabled = "OPENID_ENABLED";
}