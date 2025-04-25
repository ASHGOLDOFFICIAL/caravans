using System.Numerics;

namespace CaravansCore.Entities.Components;

public record Direction(float X, float Y) : IComponent
{
    public Direction(Vector2 vector) : this(vector.X, vector.Y) {}
    
    public Vector2 Vector => new(X, Y);
}