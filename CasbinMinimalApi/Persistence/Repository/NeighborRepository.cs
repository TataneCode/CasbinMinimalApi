using CasbinMinimalApi.Domain;
using CasbinMinimanApi.Persistence.Scissors;

namespace CasbinMinimalApi.Application.Repositories;

public class NeighborRepository(ScissorsDbContext context) : ScissorsGenericRepository<Neighbor>(context), INeighborRepository
{
    
}