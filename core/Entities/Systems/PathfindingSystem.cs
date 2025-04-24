using System.Numerics;
using CaravansCore.Entities.AI;
using CaravansCore.Entities.Components;
using CaravansCore.Level;
using CaravansCore.Utils;
using Path = CaravansCore.Entities.Components.Path;

namespace CaravansCore.Entities.Systems;

internal class PathfindingSystem(Layout level) : ISystem
{
    private readonly Pathfinder _allAllowedPathfinder = new(null);
    private readonly HashSet<Type> _requiredComponentTypes = [typeof(Position), typeof(TargetTile)];

    public void Update(EntityManager em, float deltaTime)
    {
        foreach (var entity in em.GetAllEntitiesWith(_requiredComponentTypes))
        {
            em.TryGetComponent<TargetTile>(entity, out var tile);
            if (tile is null) continue;

            em.TryGetComponent<Position>(entity, out var position);
            if (position is null) continue;

            em.TryGetComponent<Path>(entity, out var path);
            if (path is null)
            {
                path = FindPath(em, entity, position, tile);
                em.SetComponent(entity, path);
            }

            if (path.Value.Count <= 0)
            {
                em.RemoveComponent<TargetTile>(entity);
                em.RemoveComponent<Path>(entity);
                continue;
            }

            if (TileTools.NearlyEqual(position.Coordinates, path.Value.First()))
            {
                path.Value.Dequeue();
                continue;
            }

            var direction = ChooseDirection(position, path);
            em.SetComponent(entity, new Direction(direction));
            em.SetComponent(entity, new Rotation(direction));
        }
    }

    private Path FindPath(EntityManager em, Entity entity, Position pos, TargetTile tile)
    {
        var start = TileTools.NearestTile(pos.Coordinates);
        var end = tile.Tile;
        if (start == end) return new Path([]);

        em.TryGetComponent<PathPreference>(entity, out var preference);
        var path = preference is null
            ? _allAllowedPathfinder.FindPath(level, start, end)
            : new Pathfinder(preference.Tiles).FindPath(level, start, end);
        return new Path(new Queue<Point2D>(path));
    }

    private static Vector2 ChooseDirection(Position pos, Path path)
    {
        var positionValue = pos.Coordinates;
        var pathValue = path.Value;
        var next = pathValue.Peek();
        var newVector = new Vector2(next.X - positionValue.X, next.Y - positionValue.Y);
        return newVector;
    }
}