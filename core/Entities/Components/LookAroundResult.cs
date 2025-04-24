using CaravansCore.Utils;

namespace CaravansCore.Entities.Components;

internal record LookAroundResult(
    List<Entity> SeenEntities,
    List<Point2D> SeenObjects
) : IComponent;