using System.Numerics;

namespace CaravansCore.Entities.Components;

public record Velocity(float X, float Y) : IComponent
{
    public Velocity(Vector2 vector) : this(vector.X, vector.Y) {}

    public Vector2 Vector => new(X, Y);
}