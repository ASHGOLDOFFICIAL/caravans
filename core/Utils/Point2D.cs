using System.Numerics;

namespace CaravansCore.Utils;

public readonly record struct Point2D(int X, int Y)
{
    public int X { get; } = X;

    public int Y { get; } = Y;

    public Vector2 ToVector2D()
    {
        return new Vector2(X, Y);
    }

    public static Point2D operator +(Point2D a, Point2D b)
    {
        return new Point2D(a.X + b.X, a.Y + b.Y);
    }

    public static Point2D operator -(Point2D a, Point2D b)
    {
        return new Point2D(a.X - b.X, a.Y - b.Y);
    }
}