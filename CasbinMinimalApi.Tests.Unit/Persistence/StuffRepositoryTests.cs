using CasbinMinimalApi.Application.Repositories;
using CasbinMinimalApi.Domain;

namespace CasbinMinimalApi.Tests.Unit.Persistence;

public class StuffRepositoryTests : BaseRepositoryTest
{
  private readonly StuffRepository _repository;
  private readonly NeighborRepository _neighborRepository;

  public StuffRepositoryTests() : base()
  {
    _repository = new StuffRepository(dbContext);
    _neighborRepository = new NeighborRepository(dbContext);
  }

  [Fact]
  public async Task Add_Stuff_SetsCreatedOn_AndPersists()
  {
    var neighbor = CreateAndPersistNeighbor();
    var stuff = new Stuff("Mining Pickaxe", "Tool", neighbor.Id);

    _repository.Add(stuff);
    await _repository.SaveChangesAsync();

    Assert.True(stuff.Id > 0);
    Assert.True((DateTime.UtcNow - stuff.CreatedOn).TotalSeconds < 5);
    Assert.Null(stuff.ModifiedOn);
    Assert.Equal(neighbor.Id, stuff.NeighborId);
  }

  [Fact]
  public async Task GetAllAsync_ReturnsInsertedStuffs()
  {
    var neighbor = CreateAndPersistNeighbor();

    _repository.Add(new Stuff("Gold Pan", "Pan", neighbor.Id));
    _repository.Add(new Stuff("Lamp", "Oil lamp", neighbor.Id));
    await _repository.SaveChangesAsync();

    var all = await _repository.GetAllAsync();
    Assert.Equal(2, all.Count());
  }

  [Fact]
  public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
  {
    var result = await _repository.GetByIdAsync(12345);
    Assert.Null(result);
  }

  [Fact]
  public async Task Update_SetsModifiedOn()
  {
    var neighbor = CreateAndPersistNeighbor();
    var stuff = new Stuff("Hammer", "Basic", neighbor.Id);
    _repository.Add(stuff);
    await _repository.SaveChangesAsync();

    // Update description (reflection si setter privé)
    typeof(Stuff).GetProperty("Description")!.SetValue(stuff, "Heavy duty");
    _repository.Update(stuff);
    await _repository.SaveChangesAsync();

    Assert.NotNull(stuff.ModifiedOn);
    Assert.Equal("Heavy duty", stuff.Description);
  }

  [Fact]
  public async Task Delete_RemovesStuff()
  {
    var neighbor = CreateAndPersistNeighbor();
    var stuff = new Stuff("Compass", "Navigation", neighbor.Id);
    _repository.Add(stuff);
    await _repository.SaveChangesAsync();

    _repository.Delete(stuff);
    await _repository.SaveChangesAsync();

    Assert.Null(await _repository.GetByIdAsync(stuff.Id));
  }

  private Neighbor CreateAndPersistNeighbor()
  {
    var n = new Neighbor("Scrooge McDuck", "scrooge@duckburg.com");
    _neighborRepository.Add(n);
    _neighborRepository.SaveChangesAsync().GetAwaiter().GetResult();
    return n;
  }
}