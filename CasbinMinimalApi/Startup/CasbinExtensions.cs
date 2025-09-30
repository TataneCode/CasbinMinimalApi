using Casbin;
using Casbin.Persist;
using Casbin.Persist.Adapter.EFCore;
using CasbinMinimalApi.Application.Authorization;
using CasbinMinimalApi.Constants;
using CasbinMinimalApi.Infrastructure.Authorization;
using Microsoft.EntityFrameworkCore;

namespace CasbinMinimalApi.Startup;

public static class CasbinExtensions
{
  public static void ConfigureCasbin(this WebApplicationBuilder builder)
  {
    if (EF.IsDesignTime) return;

    var connectionString = builder.Configuration[ConfigurationKey.ConnectionString] ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    var options = new DbContextOptionsBuilder<CasbinDbContext<int>>()
    .UseNpgsql(connectionString)
    .Options;
    var context = new CasbinDbContext<int>(options, "casbin", "casbin_rules");
    context.Database.EnsureCreated();
    builder.Services.AddScoped(_ => context);
    var adapter = new EFCoreAdapter<int>(context);
    var policyPath = Path.Combine(builder.Environment.ContentRootPath, "Casbin", "rbac_model.conf");
    var enforcer = new Enforcer(policyPath, adapter);
    builder.Services.AddScoped<IAdapter>(_ => adapter);
    builder.Services.AddScoped<IEnforcer>(_ => enforcer);

    // Custom casbin services
    builder.Services.AddScoped<IAuthorizationService, CasbinAuthorizationService>();

    // Initialize Casbin with default roles and permissions
    builder.Services.AddScoped<IRoleService, CasbinRoleService>();
  }

  public static IApplicationBuilder UseCasbinAuthorization(this IApplicationBuilder app)
      => app.UseMiddleware<CasbinAuthorizationMiddleware>();
}
