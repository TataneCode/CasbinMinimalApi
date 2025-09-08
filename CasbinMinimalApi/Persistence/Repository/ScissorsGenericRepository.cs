using CasbinMinimalApi.Domain;
using CasbinMinimanApi.Persistence.Scissors;
using Microsoft.EntityFrameworkCore;

namespace CasbinMinimalApi.Application.Repositories;

public abstract class ScissorsGenericRepository<T>(ScissorsDbContext context) : IScissorsGenericRepository<T> where T : Entity
{

  public async Task<IEnumerable<T>> GetAllAsync()
  {
    return await context.Set<T>().ToListAsync();
  }

  public async Task<T?> GetByIdAsync(long id)
  {
    return await context.Set<T>().FindAsync(id);
  }

  public void Add(T entity)
  {
    entity.CreatedOn = DateTime.UtcNow;
    context.Set<T>().Add(entity);
  }

  public void Update(T entity)
  {
    entity.ModifiedOn = DateTime.UtcNow;
    context.Set<T>().Update(entity);
  }

  public void Delete(T entity)
  {
    context.Set<T>().Remove(entity);
  }

  public async Task SaveChangesAsync()
  {
    await context.SaveChangesAsync();
  }
}