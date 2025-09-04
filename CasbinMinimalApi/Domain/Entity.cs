namespace CasbinMinimalApi.Domain;

public abstract class Entity
{
    public long Id { get; set; }
    
    public DateTime CreatedOn { get; set; }
    
    public DateTime? ModifiedOn { get; set; }
}