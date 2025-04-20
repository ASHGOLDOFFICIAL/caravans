using AStarNavigator;
using AStarNavigator.Algorithms;
using AStarNavigator.Providers;
using CaravansCore.Level;
using CaravansCore.Terrain;
using CaravansCore.Utils;

namespace CaravansCore.Entities.AI;

internal class ExceptPathBlockedProvider(Layout level) : IBlockedProvider
{
    public bool IsBlocked(Tile coord)
    {
        return level.GetTerrain(Converters.TileToPoint(coord)) != TerrainId.Path;
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

internal class CaravanPathfinder : IPathfinder
{
    public IEnumerable<Point2D> FindPath(Layout level, Point2D start, Point2D end)
    {
        var from = Converters.Point2DToTile(start);
        var to = Converters.Point2DToTile(end);

        var navigator = new TileNavigator(
            new ExceptPathBlockedProvider(level),
            new AdjacentTilesProvider(),
            new PythagorasAlgorithm(),
            new ManhattanHeuristicAlgorithm()
        );
        return ToPoint2DPath(navigator.Navigate(from, to));
    }

    private static IEnumerable<Point2D> ToPoint2DPath(IEnumerable<Tile> tiles)
    {
        return tiles.Select(Converters.TileToPoint);
    }
}

internal static class Converters
{
    internal static Tile Point2DToTile(Point2D point)
    {
        return new Tile(point.X, point.Y);
    }

    internal static Point2D TileToPoint(Tile tile)
    {
        return new Point2D((int)tile.X, (int)tile.Y);
    }
}