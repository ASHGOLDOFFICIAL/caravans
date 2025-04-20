namespace CaravansCore.Entities.Components;

internal record CollisionResult(List<Entity> CollidedEntities) : IComponent;