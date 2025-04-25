using System.Numerics;

namespace CaravansCore.Entities.Components;

public record PlayerSubmittedPosition(float X, float Y) : IComponent
{
    public PlayerSubmittedPosition(Vector2 vector) : this(vector.X, vector.Y) {}

    public Vector2 Position => new(X, Y);
}