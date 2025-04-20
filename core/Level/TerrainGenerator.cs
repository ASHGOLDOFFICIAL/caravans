using CaravansCore.Terrain;
using CaravansCore.Utils;

namespace CaravansCore.Level;

internal class TerrainGenerator
{
    private const int PathNodes = 4;
    private readonly FastNoiseLite _noise = new();
    private readonly Random _random;

    public TerrainGenerator(int seed)
    {
        _random = new Random(seed);
        _noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        _noise.SetSeed(seed);
    }

    public Layout Generate(int width, int height)
    {
        Layout level = new(width, height);
        GenerateTerrain(level);
        GeneratePaths(level);
        return level;
    }

    private void GenerateTerrain(Layout level)
    {
        foreach (var x in Enumerable.Range(0, level.Width))
        foreach (var y in Enumerable.Range(0, level.Height))
        {
            var noiseValue = _noise.GetNoise(x, y);
            var id = noiseValue switch
            {
                > 0 => TerrainId.Grass,
                _ => TerrainId.Water
            };
            level.PlaceTerrain(id, new Point2D(x, y));
        }
    }

    private void GeneratePaths(Layout level)
    {
        List<Point2D> nodes = [];

        nodes.AddRange(from _ in Enumerable.Range(0, PathNodes)
            select _random.Next(0, level.Width)
            into x
            let y = _random.Next(0, level.Height)
            select new Point2D(x, y));

        for (var i = 0; i < nodes.Count; ++i)
        for (var j = i + 1; j < nodes.Count; ++j)
            ConnectWithPath(level, nodes[i], nodes[j]);
    }

    private static void ConnectWithPath(Layout level, Point2D source, Point2D dest)
    {
        var current = source;
        while (current != dest)
        {
            level.PlaceTerrain(TerrainId.Path, current);
            float x = dest.X - current.X;
            var absX = Math.Abs(x);
            float y = dest.Y - current.Y;
            var absY = Math.Abs(y);

            var vec = absX > absY
                ? new Point2D((int)(x / absX), 0)
                : new Point2D(0, (int)(y / absY));
            current += vec;
        }
    }
}