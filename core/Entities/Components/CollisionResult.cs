using CaravansCore.Utils;

namespace CaravansCore.Entities.Components;

internal record CollisionResult(
    List<Entity> CollidingEntities,
    List<Point2D> CollidingObjects) : IComponent;