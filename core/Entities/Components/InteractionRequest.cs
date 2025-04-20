namespace CaravansCore.Entities.Components;

public enum InteractionType
{
    Default
}

internal record InteractionRequest(
    Entity TargetEntity,
    InteractionType Type
) : IComponent;