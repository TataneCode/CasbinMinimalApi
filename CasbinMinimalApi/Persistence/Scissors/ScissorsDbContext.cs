using CasbinMinimalApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace CasbinMinimanApi.Persistence.Scissors;

public class ScissorsDbContext(DbContextOptions<ScissorsDbContext> options) : DbContext(options)
{
    public DbSet<Neighbor> Neighbors => Set<Neighbor>();

    public DbSet<Stuff> Stuffs => Set<Stuff>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("scissors");

        modelBuilder.Entity<Neighbor>().ToTable("neighbors");
        modelBuilder.Entity<Neighbor>().Property(n => n.Name).IsRequired().HasMaxLength(100);
        modelBuilder.Entity<Neighbor>().Property(n => n.Email).IsRequired().HasMaxLength(100);
        modelBuilder.Entity<Neighbor>(entity =>
        {
            entity.OwnsOne(n => n.Address, address =>
            {
                address.Property(a => a.Street).HasMaxLength(200);
                address.Property(a => a.City).HasMaxLength(50);
                address.Property(a => a.ZipCode).HasMaxLength(5);
            });
        });

        modelBuilder.Entity<Stuff>().ToTable("stuffs");
        modelBuilder.Entity<Stuff>().Property(s => s.Name).IsRequired().HasMaxLength(100);
        modelBuilder.Entity<Stuff>().Property(s => s.Description).HasMaxLength(500);
        modelBuilder.Entity<Stuff>()
            .HasOne(s => s.Neighbor)
            .WithMany()
            .HasForeignKey(s => s.NeighborId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}

