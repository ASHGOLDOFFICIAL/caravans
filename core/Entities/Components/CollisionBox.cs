namespace CaravansCore.Entities.Components;

public record CollisionBox(
    double Width,
    double Height
) : IComponent;