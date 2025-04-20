using CaravansCore.Entities;

namespace CaravansCore.Level;

public class World
{
    private const int Seed = 2;

    internal Layout Layout { get; } =
        new TerrainGenerator(Seed).Generate(64, 64);

    internal EntityManager EntityManager { get; } = new();
}