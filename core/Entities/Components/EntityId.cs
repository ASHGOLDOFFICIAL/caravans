namespace CaravansCore.Entities.Components;

public enum EntityId
{
    Player,
    Caravan
}

public record EntityType(EntityId Id) : IComponent;