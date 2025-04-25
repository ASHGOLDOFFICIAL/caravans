using CaravansCore.Entities.Components;

namespace CaravansCore.Entities.Systems;

public class EntityFollowingSystem : ISystem
{
    private readonly List<Type> _requiredComponentTypes =
        [typeof(FollowedEntity), typeof(Position)];
    
    public void Update(EntityManager em, float deltaTime)
    {
        foreach (var (entity, components) in em.GetAllEntitiesWith(_requiredComponentTypes))
        {
            var following = (FollowedEntity)components[typeof(FollowedEntity)];
            var position = (Position)components[typeof(Position)];
            
            em.TryGetComponent<Position>(following.Entity, out var secondPosition);
            if (secondPosition is null) continue;
            
            em.SetComponent(entity, new Direction(secondPosition.Coordinates - position.Coordinates));
        }
    }
}