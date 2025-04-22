using CaravansCore.Terrain;
using CaravansCore.Utils;

namespace CaravansCore.Level;

internal class TerrainGenerator
{
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
        var cities = ChooseCityLocations(level);
        var paths = MinimumSpanningTree(cities);
        GeneratePaths(level, paths);
        PlaceCities(level, cities);
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
                > -0.2f => TerrainId.Grass,
                _ => TerrainId.Water
            };
            level.PlaceTerrain(id, new Point2D(x, y));
        }
    }

    private HashSet<Point2D> ChooseCityLocations(Layout level)
    {
        var citiesAmount = _random.Next(0, 16);
        HashSet<Point2D> nodes = [];
        foreach (var _ in Enumerable.Range(0, citiesAmount))
        {
            var x = _random.Next(0, level.Width);
            var y = _random.Next(0, level.Height);
            nodes.Add(new Point2D(x, y));
        }

        return nodes;
    }

    private static HashSet<(Point2D, Point2D)> MinimumSpanningTree(HashSet<Point2D> nodes)
    {
        var unconnected = nodes
            .ToDictionary(node => node, _ => ((Point2D?)null, long.MaxValue));
        var edges = new HashSet<(Point2D, Point2D)>();

        var startNode = unconnected.Keys.First();
        unconnected[startNode] = (null, 0);

        while (unconnected.Count > 0)
        {
            var (node, (parent, _)) = unconnected.MinBy(kvp => kvp.Value.Item2);
            unconnected.Remove(node);

            if (parent is { } nonNullParent)
                edges.Add((node, nonNullParent));

            foreach (var neighbour in unconnected.Keys)
            {
                var newDist = TileTools.DistanceSquared(node, neighbour);
                if (newDist >= unconnected[neighbour].Item2) continue;
                unconnected[neighbour] = (node, newDist);
            }
        }

        return edges;
    }

    private static void GeneratePaths(Layout level, HashSet<(Point2D, Point2D)> nodes)
    {
        foreach (var (from, to) in nodes)
            ConnectWithPath(level, from, to);
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

    private static void PlaceCities(Layout level, HashSet<Point2D> nodes)
    {
        foreach (var node in nodes)
            level.PlaceTerrain(TerrainId.City, node);
    }
}