namespace CasbinMinimalApi.Startup;

public static class CasbinRoles
{
  public const string Admin = "admin";
  public const string Operator = "operator";
  public const string Viewer = "viewer";
}

public static class CasbinResources
{
  public const string Neighbor = "neighbor";
  public const string Stuff = "stuff";
}

public static class CasbinActions
{
  public const string Create = "create";
  public const string Read = "read";
  public const string Update = "update";
  public const string Delete = "delete";
}