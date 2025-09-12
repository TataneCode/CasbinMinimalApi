using CasbinMinimalApi.Application.Repositories;
using CasbinMinimalApi.Domain;

namespace CasbinMinimalApi.Tests.Unit.Persistence;

public class NeighborRepositoryTests : BaseRepositoryTest
{
  private readonly NeighborRepository _repository;

  public NeighborRepositoryTests() : base()
  {
    _repository = new NeighborRepository(dbContext);
  }

  [Fact]
  public async Task AddAsync_PersistsNeighbor_AndSetsCreatedOn()
  {
    var neighbor = new Neighbor("Scrooge McDuck", "", null);
    _repository.Add(neighbor);
    await _repository.SaveChangesAsync();

    Assert.True(neighbor.Id > 0);
    Assert.True((DateTime.UtcNow - neighbor.CreatedOn).TotalSeconds < 5);
    Assert.Null(neighbor.ModifiedOn);
  }

  [Fact]
  public async Task GetAllAsync_ReturnsInsertedItems()
  {

    var neighbor1 = new Neighbor("Scrooge McDuck", "scrooge@duckburg.com");
    var neighbor2 = new Neighbor("Donald Duck", "donald@duckburg.com");

    _repository.Add(neighbor1);
    _repository.Add(neighbor2);
    await _repository.SaveChangesAsync();

    var all = await _repository.GetAllAsync();
    Assert.Equal(2, all.Count());
  }

  [Fact]
  public async Task GetByIdAsync_ReturnsNull_WhenMissing()
  {
    var found = await _repository.GetByIdAsync(999);
    Assert.Null(found);
  }

  [Fact]
  public async Task Update_SetsModifiedOn()
  {
    var neighbor = new Neighbor("Scrooge McDuck", "scrooge@duckburg.com");
    _repository.Add(neighbor);
    await _repository.SaveChangesAsync();

    neighbor.UpdateName("Uncle Scrooge");
    _repository.Update(neighbor);
    await _repository.SaveChangesAsync();

    Assert.NotNull(neighbor.ModifiedOn);
    Assert.Equal("Uncle Scrooge", neighbor.Name);
  }

  [Fact]
  public async Task Delete_RemovesEntity()
  {
    var neighbor = new Neighbor("Scrooge McDuck", "scrooge@duckburg.com");
    _repository.Add(neighbor);
    await _repository.SaveChangesAsync();

    _repository.Delete(neighbor);
    await _repository.SaveChangesAsync();

    Assert.Null(await _repository.GetByIdAsync(neighbor.Id));
  }
}