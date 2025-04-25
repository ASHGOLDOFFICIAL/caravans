using System.Collections.Immutable;
using CaravansCore.Entities.Components;
using CaravansCore.Level;
using CaravansCore.Utils;
using Aabb = CaravansCore.Utils.Aabb;
using Vector2 = System.Numerics.Vector2;

namespace CaravansCore.Entities.Systems;

internal class CollisionDetectionSystem(Layout level) : ISystem
{
    private readonly HashSet<Type> _requiredComponentTypes =
        [typeof(Position), typeof(CollisionBox), typeof(Velocity)];

    public void Update(EntityManager em, float deltaTime)
    {
        var entities = em.GetAllEntitiesWith(_requiredComponentTypes).ToList();

        foreach (var entityA in entities)
        {
            em.TryGetComponent<Velocity>(entityA, out var velocity);
            if (velocity is null) continue;
            em.TryGetComponent<Position>(entityA, out var positionA);
            if (positionA is null) continue;
            em.TryGetComponent<CollisionBox>(entityA, out var boxA);
            if (boxA is null) continue;

            var currentAabb = new Aabb(positionA.Coordinates, boxA);

            var collidingObjects = FindCollidingObjects(currentAabb, velocity.Vector)
                .ToImmutableList();
            var collidingEntities = FindCollidingEntities(em, currentAabb, velocity.Vector)
                .ToImmutableList();

            if (collidingObjects.Count != 0 && collidingEntities.Count != 0)
                continue;
            em.SetComponent(entityA, new CollisionResult(collidingEntities, collidingObjects));
        }
    }

    private List<Point2D> FindCollidingObjects(Aabb moving, Vector2 velocity)
    {
        var timeToCollision = new Dictionary<Point2D, float>();

        var box = new CollisionBox(moving.Width, moving.Height);
        var desired = new Aabb(moving.Center + velocity, box);

        var possible = AabbCollisionTools.PossibleCollisionTiles(moving, desired);
        foreach (var tile in possible.Where(t => level.GetObject(t) is not null))
        {
            // TODO: All objects have the same collision box
            var objectBox = new CollisionBox(1, 1);
            var objectAabb = new Aabb(tile.ToVector2(), objectBox);
            var collision = AabbCollisionTools.MovingAabbOverlapsStaticAabb(
                moving, velocity, objectAabb, out var hitTime);
            if (!collision) continue;
            timeToCollision.Add(tile, hitTime);
        }

        return timeToCollision
            .OrderBy(pair => pair.Value)
            .Select(pair => pair.Key)
            .ToList();
    }

    private static List<Entity> FindCollidingEntities(EntityManager em, Aabb moving, Vector2 velocity)
    {
        var timeToCollision = new Dictionary<Entity, float>();

        var box = new CollisionBox(moving.Width, moving.Height);
        var desired = new Aabb(moving.Center + velocity, box);

        var collisionArea = AabbCollisionTools.PossibleCollisionArea(moving, desired);

        foreach (var entity in em.GetAllEntitiesWith<CollisionBox>())
        {
            em.TryGetComponent<Position>(entity, out var position);
            if (position is null) continue;
            em.TryGetComponent<CollisionBox>(entity, out var entityBox);
            if (entityBox is null) continue;
            var entityAabb = new Aabb(position.Coordinates, entityBox);

            var collisionPossible = AabbCollisionTools
                .AabbOverlapsAabb(collisionArea, entityAabb);
            if (!collisionPossible) continue;

            var collision = AabbCollisionTools.MovingAabbOverlapsStaticAabb(
                moving, velocity, entityAabb, out var hitTime);

            if (!collision) continue;
            timeToCollision.Add(entity, hitTime);
        }

        return timeToCollision
            .OrderBy(pair => pair.Value)
            .Select(pair => pair.Key)
            .ToList();
    }
}