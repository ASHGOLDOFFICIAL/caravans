using System.Numerics;
using CaravansCore.Utils;
using GodotVector2 = Godot.Vector2;

namespace CaravansView.Utils;

public static class Converter
{
    public static GodotVector2 ToGodotVector(Vector2 vector)
    {
        var x = vector.X;
        var y = vector.Y;
        return new GodotVector2(x, y);
    }

    public static GodotVector2 ToGodotVector(Point2D tilePosition)
    {
        var x = tilePosition.X;
        var y = tilePosition.Y;
        return new GodotVector2(x, y);
    }

    public static Vector2 ToSystemVector(GodotVector2 vector)
    {
        return new Vector2(vector.X, vector.Y);
    }
}