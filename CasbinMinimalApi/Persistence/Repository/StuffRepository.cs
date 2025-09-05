using CasbinMinimalApi.Domain;
using CasbinMinimanApi.Persistence.Scissors;

namespace CasbinMinimalApi.Application.Repositories;

public class StuffRepository(ScissorsDbContext context) : ScissorsGenericRepository<Stuff>(context), IStuffRepository
{
    
}