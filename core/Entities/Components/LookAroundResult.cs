using System.Collections.Immutable;
using CaravansCore.Utils;

namespace CaravansCore.Entities.Components;


internal record LookAroundResult(
    ImmutableList<Entity> SeenEntities,
    ImmutableList<Point2D> SeenObjects
) : IComponent;