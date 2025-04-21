using CaravansCore.Entities.Components;
using CaravansCore.Utils;

namespace CaravansCore.Entities.Systems;

public class CollisionDetectionSystem : ISystem
{
    private readonly HashSet<Type> _requiredComponentTypes = [typeof(Position), typeof(CollisionBox)];

    public void Update(EntityManager em, float deltaTime)
    {
        var entities = em.GetAllEntitiesWith(_requiredComponentTypes).ToList();
        var collisions = new Dictionary<Entity, List<Entity>>();

        for (var i = 0; i < entities.Count; ++i)
        {
            var entityA = entities[i];
            em.TryGetComponent<Position>(entityA, out var positionA);
            if (positionA is null) continue;
            em.TryGetComponent<CollisionBox>(entityA, out var boxA);
            if (boxA is null) continue;

            for (var j = i + 1; j < entities.Count; ++j)
            {
                var entityB = entities[j];
                em.TryGetComponent<Position>(entityB, out var positionB);
                if (positionB is null) continue;
                em.TryGetComponent<CollisionBox>(entityB, out var boxB);
                if (boxB is null) continue;
                
                if (!CollisionTools.AabbOverlap(positionA, boxA, positionB, boxB))
                    continue;

                if (!collisions.TryAdd(entityA, [entityB]))
                    collisions[entityA].Add(entityB);
                if (!collisions.TryAdd(entityB, [entityA]))
                    collisions[entityB].Add(entityA);
            }
        }

        foreach (var (entity, collided) in collisions)
            em.SetComponent(entity, new CollisionResult(collided));
    }
}