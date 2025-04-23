using AStarNavigator;
using AStarNavigator.Algorithms;
using AStarNavigator.Providers;
using CaravansCore.Level;
using CaravansCore.Level.Content;
using CaravansCore.Utils;

namespace CaravansCore.Entities.AI;

internal class NoBlockedProvider : IBlockedProvider
{
    public bool IsBlocked(Tile coord)
    {
        return false;
    }
}

internal class OnlyAllowedBlockedProvider(Layout level, HashSet<TerrainId> allowed) : IBlockedProvider
{
    public bool IsBlocked(Tile coord)
    {
        return !IsAllowed(coord);
    }

    private bool IsAllowed(Tile coord)
    {
        return allowed.Any(terrain => level.GetTerrain(Converters.ToPoint2D(coord)) == terrain);
    }
}

internal class AdjacentTilesProvider : INeighborProvider
{
    private static readonly (double, double)[] Directions =
    [
        (0.0, -1.0),
        (1.0, 0.0),
        (0.0, 1.0),
        (-1.0, 0.0)
    ];

    public IEnumerable<Tile> GetNeighbors(Tile tile)
    {
        return Directions.Select(direction =>
            new Tile(tile.X + direction.Item1, tile.Y + direction.Item2));
    }
}

internal class Pathfinder(HashSet<TerrainId>? allowed)
{
    public IEnumerable<Point2D> FindPath(Layout level, Point2D start, Point2D end)
    {
        var from = Converters.ToTile(start);
        var to = Converters.ToTile(end);

        IBlockedProvider blocker = allowed is null
            ? new NoBlockedProvider()
            : new OnlyAllowedBlockedProvider(level, allowed);

        var navigator = new TileNavigator(
            blocker,
            new AdjacentTilesProvider(),
            new PythagorasAlgorithm(),
            new ManhattanHeuristicAlgorithm()
        );

        return ToPoint2DPath(navigator.Navigate(from, to));
    }

    private static IEnumerable<Point2D> ToPoint2DPath(IEnumerable<Tile> tiles)
    {
        return tiles.Select(Converters.ToPoint2D);
    }
}

internal static class Converters
{
    internal static Tile ToTile(Point2D point)
    {
        return new Tile(point.X, point.Y);
    }

    internal static Point2D ToPoint2D(Tile tile)
    {
        return new Point2D((int)tile.X, (int)tile.Y);
    }
}