using CaravansCore.Entities.Components.Types;

namespace CaravansCore.Entities.Components;

internal record InteractionRequest(
    Entity TargetEntity,
    InteractionType Type
) : IComponent;