using System.Numerics;

namespace CaravansCore.Entities.Components;

public record CollisionBox(float Width, float Height) : IComponent
{
    public Vector2 Size => new(Width, Height);
}
