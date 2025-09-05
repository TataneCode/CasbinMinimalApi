using CasbinMinimalApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace CasbinMinimanApi.Persistence.Scissors;

public static class SeedDataExtensions
{
  public static async Task SeedDataAsync(this ScissorsDbContext context)
  {
    if (await context.Neighbors.AnyAsync()) return;

    var scrooge = new Neighbor(
        "Scrooge McDuck",
        "scrooge@duckburg.com",
        new Address("1 Money Bin Hill", "Duckburg", "10000"));

    var donald = new Neighbor(
        "Donald Duck",
        "donald@duckburg.com",
        new Address("1313 Webfoot Walk", "Duckburg", "10001"));

    var gyro = new Neighbor(
        "Gyro Gearloose",
        "gyro@duckburg.com",
        new Address("42 Workshop Lane", "Duckburg", "10002"));

    var gladstone = new Neighbor(
        "Gladstone Gander",
        "gladstone@duckburg.com",
        new Address("7 Clover Street", "Duckburg", "10003"));

    var huey = new Neighbor(
        "Huey Duck",
        "huey@duckburg.com",
        new Address("1313 Webfoot Walk", "Duckburg", "10001"));

    await context.Neighbors.AddRangeAsync(scrooge, donald, gyro, gladstone, huey);
    await context.SaveChangesAsync();

    var stuffs = new List<Stuff>
        {
            // Scrooge (3)
            new("Gold Pan", "Used for panning river gold", scrooge.Id),
            new("Mining Pickaxe", "Sturdy pick for treasure digs", scrooge.Id),
            new("Accounting Ledger", "Tracks every dime", scrooge.Id),

            // Donald (2)
            new("Fishing Rod", "Weekend lake fishing rod", donald.Id),
            new("Claw Hammer", "Standard carpentry hammer", donald.Id),

            // Gyro (3)
            new("Prototype Wrench", "Adjustable experimental wrench", gyro.Id),
            new("Soldering Iron", "For fine electronic repairs", gyro.Id),
            new("Gizmo Toolkit", "Custom toolkit for inventions", gyro.Id),

            // Gladstone (1)
            new("Lucky Screwdriver", "Always fits the first try", gladstone.Id),

            // Huey (2)
            new("Junior Woodchucks Compass", "Field navigation tool", huey.Id),
            new("Camping Hatchet", "Light hatchet for camp prep", huey.Id)
        };

    await context.Stuffs.AddRangeAsync(stuffs);
    await context.SaveChangesAsync();
  }
}