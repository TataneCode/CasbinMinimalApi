namespace CasbinMinimalApi.Domain;

public sealed class Neighbor : Entity
{
    public string Name { get; private set; }
    
    public string Email { get; private set; }
    
    public Address? Address { get; private set; }

    private Neighbor()
    {
        Name = string.Empty;
        Email = string.Empty;
        Address = null;
    }
    
    public Neighbor(string name, string email, Address? address = null)
    {
        Name = name;
        Email = email;
        Address = address;
    }
    
    public void UpdateAddress(Address? address)
    {
        Address = address;
    }
}

public sealed class Address
{
    public string Street { get; }
    
    public string City { get; }
    
    public string ZipCode { get; }

    private Address()
    {
        Street = string.Empty;
        City = string.Empty;
        ZipCode = string.Empty;
    }

    public Address(string street, string city, string zipCode)
    {
        Street = street;
        City = city;
        ZipCode = zipCode;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Address other) return false;
        return other.Street == Street && other.City == City && other.ZipCode == ZipCode;
    }
    
    public override int GetHashCode() => HashCode.Combine(Street, City, ZipCode);
}