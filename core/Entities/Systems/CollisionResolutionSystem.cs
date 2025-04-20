using CaravansCore.Entities.Components;

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
                FixOverlap(em, entity, second);
        }
    }

    private static void FixOverlap(EntityManager em, Entity entityA, Entity entityB)
    {
        em.TryGetComponent<Position>(entityA, out var position);
        if (position is null) return;
        em.TryGetComponent<Velocity>(entityA, out var velocity);
        if (velocity is null) return;
        em.TryGetComponent<Abilities>(entityA, out var abilities);
        if (abilities is null) return;
        // TODO
    }
}