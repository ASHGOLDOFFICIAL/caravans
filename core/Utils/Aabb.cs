using System.Numerics;
using CaravansCore.Entities.Components;

namespace CaravansCore.Utils;

public record Aabb()
{
    public Aabb(Vector2 center, CollisionBox box) : this()
    {
        MinX = center.X - box.Width / 2;
        MaxX = center.X + box.Width / 2;
        MinY = center.Y - box.Height / 2;
        MaxY = center.Y + box.Height / 2;
    }

    public Aabb(Vector2 origin, Vector2 size) : this()
    {
        MinX = origin.X;
        MaxX = origin.X + size.X;
        MinY = origin.Y;
        MaxY = origin.Y + size.Y;
    }

    public float MinX { get; }
    public float MaxX { get; }
    public float MinY { get; }
    public float MaxY { get; }

    public Vector2 Center => new(MinX + Width / 2, MinY + Height / 2);
    public Vector2 Origin => new(MinX, MinY);

    public float Width => MaxX - MinX;
    public float Height => MaxY - MinY;
    public Vector2 Dimensions => new(Width, Height);
}