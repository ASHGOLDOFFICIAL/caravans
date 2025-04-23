using CaravansCore.Entities.Components;
using CaravansCore.Utils;
using Aabb = CaravansCore.Utils.Aabb;

namespace CaravansCore.Entities.Systems;

public class CollisionResolutionSystem : ISystem
{
    public void Update(EntityManager em, float deltaTime)
    {
        foreach (var entity in em.GetAllEntitiesWith<CollisionResult>())
        {
            em.TryGetComponent<CollisionResult>(entity, out var collided);
            if (collided is null) continue;
            foreach (var point in collided.CollidingObjects)
                ResolveObjectCollision(em, entity, point);
            foreach (var entityB in collided.CollidingEntities)
                ResolveEntityCollision(em, entity, entityB);
            em.RemoveComponent<CollisionResult>(entity);
        }
    }

    private static void ResolveObjectCollision(EntityManager em, Entity entity, Point2D point)
    {
        em.TryGetComponent<Velocity>(entity, out var desired);
        if (desired is null) return;
        em.TryGetComponent<Position>(entity, out var position);
        if (position is null) return;
        em.TryGetComponent<CollisionBox>(entity, out var box);
        if (box is null) return;

        var velocity = desired.Vector;
        var aabb = new Aabb(position.Coordinates, box);
        var objectAabb = new Aabb(point.ToVector2(), new CollisionBox(1, 1));
        var clamped = AabbCollisionTools
            .MovingAabbOverlapResolution(aabb, velocity, objectAabb);
        em.SetComponent(entity, new Velocity(clamped));
    }

    private static void ResolveEntityCollision(EntityManager em, Entity entityA, Entity entityB)
    {
        em.TryGetComponent<Velocity>(entityA, out var desired);
        if (desired is null) return;
        em.TryGetComponent<Position>(entityA, out var position);
        if (position is null) return;
        em.TryGetComponent<CollisionBox>(entityA, out var box);
        if (box is null) return;

        em.TryGetComponent<Position>(entityB, out var positionB);
        if (positionB is null) return;
        em.TryGetComponent<CollisionBox>(entityB, out var boxB);
        if (boxB is null) return;

        var velocity = desired.Vector;

        var aAabb = new Aabb(position.Coordinates, box);
        var bAabb = new Aabb(positionB.Coordinates, boxB);

        var clamped = AabbCollisionTools
            .MovingAabbOverlapResolution(aAabb, velocity, bAabb);
        em.SetComponent(entityA, new Velocity(clamped));
    }
}