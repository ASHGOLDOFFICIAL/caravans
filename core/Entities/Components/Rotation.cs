using System.Numerics;

namespace CaravansCore.Entities.Components;

public record Rotation(float X, float Y) : IComponent
{
    public Rotation(Vector2 vector) : this(vector.X, vector.Y) {}

    public Vector2 Direction => new(X, Y);
}