using System.Numerics;
using CaravansCore.Entities.Components;
using CaravansCore.Utils;

namespace CaravansCore.Entities.Systems;

public class CollisionResolutionSystem : ISystem
{
    public void Update(EntityManager em, float deltaTime)
    {
        foreach (var entity in em.GetAllEntitiesWith<CollisionResult>())
        {
            em.TryGetComponent<CollisionResult>(entity, out var collided);
            if (collided is null) continue;
            foreach (var second in collided.CollidedEntities)
                ResolveCollision(em, entity, second);
        }
    }

    private static void ResolveCollision(EntityManager em, Entity entityA, Entity entityB)
    {
        em.TryGetComponent<Position>(entityA, out var positionA);
        if (positionA is null) return;
        em.TryGetComponent<CollisionBox>(entityA, out var boxA);
        if (boxA is null) return;
        em.TryGetComponent<Position>(entityB, out var positionB);
        if (positionB is null) return;
        em.TryGetComponent<CollisionBox>(entityB, out var boxB);
        if (boxB is null) return;

        if (!CollisionTools.AabbOverlap(positionA, boxA, positionB, boxB))
            return;

        var aMinX = positionA.Value.X - boxA.Width / 2;
        var aMaxX = positionA.Value.X + boxA.Width / 2;
        var aMinY = positionA.Value.Y - boxA.Height / 2;
        var aMaxY = positionA.Value.Y + boxA.Height / 2;

        var bMinX = positionB.Value.X - boxB.Width / 2;
        var bMaxX = positionB.Value.X + boxB.Width / 2;
        var bMinY = positionB.Value.Y - boxB.Height / 2;
        var bMaxY = positionB.Value.Y + boxB.Height / 2;

        var overlapX = Math.Min(aMaxX - bMinX, bMaxX - aMinX);
        var overlapXVec = new Vector2(overlapX, 0);
        var overlapY = Math.Min(aMaxY - bMinY, bMaxY - aMinY);
        var overlapYVec = new Vector2(0, overlapY);

        em.TryGetComponent<Velocity>(entityA, out var velocityA);
        em.TryGetComponent<Velocity>(entityB, out var velocityB);

        SeparateEntities(
            em, entityA, entityB,
            positionA, positionB,
            velocityA, velocityB,
            overlapX < overlapY ? overlapXVec : overlapYVec, 
            overlapX < overlapY ? aMaxX > bMaxX : aMaxY > bMaxY);
    }

    private static void SeparateEntities(
        EntityManager em,
        Entity entityA,
        Entity entityB,
        Position positionA,
        Position positionB,
        Velocity? velocityA,
        Velocity? velocityB,
        Vector2 shift,
        bool positiveShift)
    {
        if (velocityA is not null && velocityB is null)
        {
            var newPos = ShiftPosition(positionA, shift, positiveShift);
            em.SetComponent(entityA, newPos);
        }
        else if (velocityB is not null && velocityA is null)
        {
            var newPos = ShiftPosition(positionB, shift, positiveShift);
            em.SetComponent(entityB, newPos);
        }
        else
        {
            var halfShift = shift / 2;
            var newPosA = ShiftPosition(positionA, halfShift, positiveShift);
            var newPosB = ShiftPosition(positionB, halfShift, !positiveShift);
            em.SetComponent(entityA, newPosA);
            em.SetComponent(entityB, newPosB);
        }
    }

    private static Position ShiftPosition(Position position, Vector2 shift, bool positiveShift)
    {
        var newPositionVec = position.Value;
        if (positiveShift)
            newPositionVec += shift;
        else
            newPositionVec -= shift;
        return new Position(newPositionVec);
    }
}