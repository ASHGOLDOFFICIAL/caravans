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
}