using System.Collections.Immutable;
using CaravansCore.Utils;

namespace CaravansCore.Entities.Components;

internal record CollisionResult(
    ImmutableList<Entity> CollidingEntities,
    ImmutableList<Point2D> CollidingObjects
) : IComponent;