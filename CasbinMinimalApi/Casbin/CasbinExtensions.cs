using Casbin;
using Casbin.Persist;
using Casbin.Persist.Adapter.EFCore;
using Microsoft.EntityFrameworkCore;

namespace CasbinMinimalApi.Casbin;

public static class CasbinExtensions
{

  public static void ConfigureCasbin(this WebApplicationBuilder builder)
  {
    // Casbin - start
    var connectionString = builder.Configuration["PG_CONNECTION_STRING"] ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
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
    builder.Services.AddScoped<IInitializationService, CasbinInitializationService>();
    // Casbin - end
  }
}