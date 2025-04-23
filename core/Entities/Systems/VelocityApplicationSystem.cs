using CaravansCore.Entities.Components;

namespace CaravansCore.Entities.Systems;

public class VelocityApplicationSystem : ISystem
{
    private readonly HashSet<Type> _requiredComponentTypes =
        [typeof(Position), typeof(Velocity)];

    public void Update(EntityManager em, float deltaTime)
    {
        foreach (var entity in em.GetAllEntitiesWith(_requiredComponentTypes))
        {
            em.TryGetComponent<Position>(entity, out var position);
            if (position is null) continue;
            em.TryGetComponent<Velocity>(entity, out var velocity);
            if (velocity is null) continue;

            var newPosition = new Position(position.Coordinates + velocity.Vector);
            em.SetComponent(entity, newPosition);
            em.RemoveComponent<Velocity>(entity);
            em.RemoveComponent<Direction>(entity);
        }
    }
}