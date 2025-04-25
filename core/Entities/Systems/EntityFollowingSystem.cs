using CaravansCore.Entities.Components;

namespace CaravansCore.Entities.Systems;

public class EntityFollowingSystem : ISystem
{
    private readonly HashSet<Type> _requiredComponentTypes =
        [typeof(FollowedEntity), typeof(Position)];
    
    public void Update(EntityManager em, float deltaTime)
    {
        foreach (var entity in em.GetAllEntitiesWith(_requiredComponentTypes))
        {
            em.TryGetComponent<FollowedEntity>(entity, out var following);
            if (following is null) continue;
            
            em.TryGetComponent<Position>(entity, out var position);
            if (position is null) continue;
            
            em.TryGetComponent<Position>(following.Entity, out var secondPosition);
            if (secondPosition is null) continue;
            
            em.SetComponent(entity, new Direction(secondPosition.Coordinates - position.Coordinates));
        }
    }
}