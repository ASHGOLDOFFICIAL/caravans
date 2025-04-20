using System.Numerics;

namespace CaravansCore.Utils;

public static class TileTools
{
    public static bool NearlyEqual(Vector2 floatPosition, Point2D intPosition)
    {
        return Math.Abs(floatPosition.X - intPosition.X) + Math.Abs(floatPosition.Y - intPosition.Y) < 0.125d;
    }

    public static Point2D NearestTile(Vector2 realPosition)
    {
        var tileX = (int)(realPosition.X + 0.5);
        var tileY = (int)(realPosition.Y + 0.5);
        return new Point2D(tileX, tileY);
    }
}