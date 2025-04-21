namespace CaravansCore.Entities.Components;

public record CollisionBox(
    float Width,
    float Height
) : IComponent;