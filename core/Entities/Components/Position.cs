using System.Numerics;

namespace CaravansCore.Entities.Components;

public record Position(float X, float Y) : IComponent
{
    public Position(Vector2 vector) : this(vector.X, vector.Y) {}

    public Vector2 Coordinates => new(X, Y);
}