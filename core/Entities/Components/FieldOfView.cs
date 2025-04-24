namespace CaravansCore.Entities.Components;

public record FieldOfView(
    float Radius,
    byte Angle
) : IComponent;
