using System.Collections.Immutable;
using System.Numerics;
using CaravansCore.Entities.AI;
using CaravansCore.Entities.Components;
using CaravansCore.Level;
using CaravansCore.Utils;
using Path = CaravansCore.Entities.Components.Path;

namespace CaravansCore.Entities.Systems;

internal class PathfindingSystem(Layout level) : ISystem
{
    private readonly TileBasedPathfinder _allAllowedTileBasedPathfinder = new(null);
    private readonly List<Type> _requiredComponentTypes = [typeof(Position), typeof(TargetPosition)];

    public void Update(EntityManager em, float deltaTime)
    {
        foreach (var (entity, components) in em.GetAllEntitiesWith(_requiredComponentTypes))
        {
            var position = (Position)components[typeof(Position)];
            var tile = (TargetPosition)components[typeof(TargetPosition)];

            em.TryGetComponent<Path>(entity, out var path);
            if (path is null)
            {
                path = FindPath(em, entity, position, tile);
                em.SetComponent(entity, path);
            }

            if (path.Tiles.IsEmpty)
            {
                em.RemoveComponent<TargetPosition>(entity);
                em.RemoveComponent<Path>(entity);
                continue;
            }

            if (TileTools.NearlyEqual(position.Coordinates, path.Tiles.First()))
            {
                em.SetComponent(entity, new Path(path.Tiles.Dequeue()));
                continue;
            }

            var direction = ChooseDirection(position, path);
            em.SetComponent(entity, new Direction(direction));
            em.SetComponent(entity, new Rotation(direction));
            em.SetComponent(entity, new MoveIntent());
        }
    }

    private Path FindPath(EntityManager em, Entity entity, Position pos, TargetPosition position)
    {
        var start = TileTools.NearestTile(pos.Coordinates);
        var end = TileTools.NearestTile(position.Tile);
        if (start == end) return new Path([]);

        em.TryGetComponent<PathPreference>(entity, out var preference);
        var path = preference is null
            ? _allAllowedTileBasedPathfinder.FindPath(level, start, end)
            : new TileBasedPathfinder(preference.Tiles).FindPath(level, start, end);
        return new Path(ImmutableQueue.CreateRange(path));
    }

    private static Vector2 ChooseDirection(Position pos, Path path)
    {
        var positionValue = pos.Coordinates;
        var pathValue = path.Tiles;
        var next = pathValue.Peek();
        var newVector = new Vector2(next.X - positionValue.X, next.Y - positionValue.Y);
        return newVector;
    }
}