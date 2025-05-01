namespace CaravansCore.Entities.Components;

internal record Died(Entity? Killer) : IComponent;