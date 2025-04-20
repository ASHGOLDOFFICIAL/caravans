using System.Numerics;

namespace CaravansCore.Utils;

public static class TileTools
{
    public static bool NearlyEqual(Vector2 floatPosition, Point2D intPosition)
    {
        return Math.Abs(floatPosition.X - intPosition.X) + Math.Abs(floatPosition.Y - intPosition.Y) < 0.25;
    }

    public static Point2D NearestTile(Vector2 realPosition)
    {
        return new Point2D((int)realPosition.X, (int)realPosition.Y);
    }
}