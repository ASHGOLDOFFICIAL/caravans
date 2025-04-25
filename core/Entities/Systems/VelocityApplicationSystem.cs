using CaravansCore.Entities.Components;

namespace CaravansCore.Entities.Systems;

public class VelocityApplicationSystem : ISystem
{
    private readonly List<Type> _requiredComponentTypes =
        [typeof(Position), typeof(Velocity)];

    public void Update(EntityManager em, float deltaTime)
    {
        foreach (var (entity, components) in em.GetAllEntitiesWith(_requiredComponentTypes))
        {
            var position = (Position)components[typeof(Position)];
            var velocity = (Velocity)components[typeof(Velocity)];

            var newPosition = new Position(position.Coordinates + velocity.Vector);
            em.SetComponent(entity, newPosition);
            em.RemoveComponent<Velocity>(entity);
            em.RemoveComponent<Direction>(entity);
        }
    }
}