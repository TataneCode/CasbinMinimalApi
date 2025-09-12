using CasbinMinimanApi.Persistence.Scissors;
using Microsoft.EntityFrameworkCore;

namespace CasbinMinimalApi.Tests.Unit.Persistence;

[Collection("database-collection")]
public abstract class BaseRepositoryTest : IAsyncLifetime
{
  protected readonly ScissorsDbContext dbContext;

  protected BaseRepositoryTest()
  {
    var options = new DbContextOptionsBuilder<ScissorsDbContext>()
        .UseInMemoryDatabase("meet-my-scissors")
        .EnableSensitiveDataLogging(true)
        .Options;

    dbContext = new ScissorsDbContext(options);
  }

  public async Task DisposeAsync()
  {
    dbContext.RemoveRange(dbContext.Stuffs);
    dbContext.RemoveRange(dbContext.Neighbors);
    await dbContext.SaveChangesAsync();
  }

  public async Task InitializeAsync()
  {
    await dbContext.Database.EnsureCreatedAsync();
  }
}