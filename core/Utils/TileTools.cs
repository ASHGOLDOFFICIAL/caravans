using Vector2 = System.Numerics.Vector2;

namespace CaravansCore.Utils;

public static class TileTools
{
    public static bool NearlyEqual(Vector2 floatPosition, Point2D intPosition)
    {
        return Math.Abs(floatPosition.X - intPosition.X) + Math.Abs(floatPosition.Y - intPosition.Y) < 0.125d;
    }

    public static Point2D NearestTile(Vector2 realPosition)
    {
        var tileX = (int)Math.Round(realPosition.X);
        var tileY = (int)Math.Round(realPosition.Y);
        return new Point2D(tileX, tileY);
    }

    public static long DistanceSquared(Point2D from, Point2D to)
    {
        var deltaX = to.X - from.X;
        var deltaY = to.Y - from.Y;
        return deltaX * deltaX + deltaY * deltaY;
    }

    public static IEnumerable<Point2D> Neighbors(Point2D point)
    {
        return Enumerable.Range(-1, 3)
            .SelectMany(dx => Enumerable
                .Range(-1, 3)
                .Select(dy => (dx, dy)))
            .Where(d => d.dx != 0 || d.dy != 0)
            .Select(d => new Point2D(point.X + d.dx, point.Y + d.dy));
    }
}