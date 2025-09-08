using System.Security.Claims;
using Casbin;
using Casbin.Model;
using Microsoft.AspNetCore.Http;

namespace CasbinMinimalApi.Infrastructure.Authorization;

public class CasbinAuthorizationServiceTest
{
  private static IEnforcer CreateEnforcer()
  {
    // Modèle RBAC de base
    const string modelText = """
[request_definition]
r = sub, obj, act

[policy_definition]
p = sub, obj, act

[role_definition]
g = _, _

[policy_effect]
e = some(where (p.eft == allow))

[matchers]
m = r.sub == p.sub && r.obj == p.obj && r.act == p.act
""";
    var model = DefaultModel.CreateFromText(modelText);
    // Pas d'adapter => in-memory pur pour les tests
    var enforcer = new Enforcer(model);
    return enforcer;
  }

  private static IHttpContextAccessor BuildContextWithEmail(string? email)
  {
    var accessor = new HttpContextAccessor();
    var ctx = new DefaultHttpContext();
    if (!string.IsNullOrWhiteSpace(email))
    {
      var principal = new ClaimsPrincipal(
        new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, email) }, "TestAuth"));
      ctx.User = principal;
    }
    accessor.HttpContext = ctx;
    return accessor;
  }

  [Fact]
  public async Task HasPermissionAsync_ReturnsTrue_WhenPolicyMatches()
  {
    var enforcer = CreateEnforcer();
    enforcer.AddPolicy("user@example.com", "resource", "action");

    var service = new CasbinAuthorizationService(enforcer, BuildContextWithEmail("user@example.com"));

    var result = await service.HasPermissionAsync("resource", "action", "user@example.com");

    Assert.True(result);
  }

  [Fact]
  public async Task HasPermissionAsync_ReturnsFalse_WhenNoPolicy()
  {
    var enforcer = CreateEnforcer();
    var service = new CasbinAuthorizationService(enforcer, BuildContextWithEmail("user@example.com"));

    var result = await service.HasPermissionAsync("resource", "action", "user@example.com");

    Assert.False(result);
  }

  [Fact]
  public async Task AddRoleForUserAsync_AssignsRole()
  {
    var enforcer = CreateEnforcer();
    // Ajout d'une permission pour le rôle
    enforcer.AddPolicy("admin", "stuff", "create");

    var service = new CasbinAuthorizationService(enforcer, BuildContextWithEmail("admin@site.local"));

    var added = await service.AddRoleForUserAsync("admin@site.local", "admin");
    Assert.True(added);
    Assert.Contains("admin", enforcer.GetRolesForUser("admin@site.local"));
  }

  [Fact]
  public async Task RemoveRoleForUserAsync_RemovesRole()
  {
    var enforcer = CreateEnforcer();
    enforcer.AddPolicy("admin", "neighbor", "read");
    enforcer.AddGroupingPolicy("admin@site.local", "admin");

    var service = new CasbinAuthorizationService(enforcer, BuildContextWithEmail("admin@site.local"));

    var removed = await service.RemoveRoleForUserAsync("admin@site.local", "admin");
    Assert.True(removed);
    Assert.Empty(enforcer.GetRolesForUser("admin@site.local"));
  }

  [Fact]
  public void GetRolesForUserAsync_ReturnsRoles()
  {
    var enforcer = CreateEnforcer();
    enforcer.AddGroupingPolicy("user@x.com", "viewer");
    enforcer.AddGroupingPolicy("user@x.com", "operator");

    var service = new CasbinAuthorizationService(enforcer, BuildContextWithEmail("user@x.com"));

    var roles = service.GetRolesForUserAsync("user@x.com");
    Assert.Equal(["operator", "viewer"], roles.OrderBy(r => r));
  }

  [Fact]
  public void GetUsersForRoleAsync_ReturnsUsers()
  {
    var enforcer = CreateEnforcer();
    enforcer.AddGroupingPolicy("a@x.com", "viewer");
    enforcer.AddGroupingPolicy("b@x.com", "viewer");

    var service = new CasbinAuthorizationService(enforcer, BuildContextWithEmail(null));

    var users = service.GetUsersForRoleAsync("viewer");
    Assert.Equal(["a@x.com", "b@x.com"], users.OrderBy(u => u));
  }

  [Fact]
  public async Task AddPermissionForRoleAsync_AddsPolicy()
  {
    var enforcer = CreateEnforcer();
    var service = new CasbinAuthorizationService(enforcer, BuildContextWithEmail(null));

    var ok = await service.AddPermissionForRoleAsync("operator", "stuff", "update");
    Assert.True(ok);
    Assert.Contains(enforcer.GetPolicy(), p => p.SequenceEqual(["operator", "stuff", "update"]));
  }

  [Fact]
  public async Task RemovePermissionForRoleAsync_RemovesPolicy()
  {
    var enforcer = CreateEnforcer();
    enforcer.AddPolicy("operator", "stuff", "update");

    var service = new CasbinAuthorizationService(enforcer, BuildContextWithEmail(null));

    var ok = await service.RemovePermissionForRoleAsync("operator", "stuff", "update");
    Assert.True(ok);
    Assert.DoesNotContain(enforcer.GetPolicy(), p => p.SequenceEqual(["operator", "stuff", "update"]));
  }

  [Fact]
  public void HasCurrentUserRole_ReturnsTrue_WhenRolePresent()
  {
    var enforcer = CreateEnforcer();
    enforcer.AddGroupingPolicy("user@example.com", "admin");

    var service = new CasbinAuthorizationService(enforcer, BuildContextWithEmail("user@example.com"));

    Assert.True(service.HasCurrentUserRole("admin"));
  }

  [Fact]
  public void HasCurrentUserAnyRole_ReturnsTrue_OnIntersect()
  {
    var enforcer = CreateEnforcer();
    enforcer.AddGroupingPolicy("user@example.com", "viewer");

    var service = new CasbinAuthorizationService(enforcer, BuildContextWithEmail("user@example.com"));

    Assert.True(service.HasCurrentUserAnyRole("admin", "viewer"));
  }

  [Fact]
  public void HasCurrentUserRole_ReturnsFalse_WhenNoEmail()
  {
    var enforcer = CreateEnforcer();
    var service = new CasbinAuthorizationService(enforcer, BuildContextWithEmail(null));
    Assert.False(service.HasCurrentUserRole("admin"));
  }

  [Fact]
  public void HasCurrentUserAnyRole_ReturnsFalse_WhenNoEmail()
  {
    var enforcer = CreateEnforcer();
    var service = new CasbinAuthorizationService(enforcer, BuildContextWithEmail(null));
    Assert.False(service.HasCurrentUserAnyRole("admin", "viewer"));
  }
}