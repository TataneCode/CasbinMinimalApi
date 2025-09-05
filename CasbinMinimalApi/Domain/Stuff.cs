namespace CasbinMinimalApi.Domain;

public sealed class Stuff : Entity
{
    public string Name { get; private set; }
    
    public string Description { get; private set; }
    
    public Neighbor? Neighbor { get; private set; }
    
    public long  NeighborId { get; private set; }

    private Stuff()
    {
        Name = string.Empty;
        Description = string.Empty;
        Neighbor = null;
    }
    
    public Stuff(string name, string description, long neighborId)
    {
        Name = name;
        Description = description;
        NeighborId = neighborId; 
    }
}