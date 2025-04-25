using CaravansCore.Entities.Components;
using Vector2 = System.Numerics.Vector2;

namespace CaravansCore.Entities.Systems;

internal class VelocityCalculationSystem : ISystem
{
    private readonly List<Type> _requiredComponentTypes =
        [typeof(Direction), typeof(Abilities)];

    public void Update(EntityManager em, float deltaTime)
    {
        foreach (var (entity, components) in em.GetAllEntitiesWith(_requiredComponentTypes))
        {
            var direction = (Direction)components[typeof(Direction)];
            if (direction.Vector == Vector2.Zero) continue;
            
            var abilities = (Abilities)components[typeof(Abilities)];

            var velocity = CalculateVelocity(direction, abilities, deltaTime);
            em.SetComponent(entity, velocity);
        }

        foreach (var (player, submitted) in em.GetAllEntitiesWith<PlayerSubmittedPosition>())
        {
            em.TryGetComponent<Position>(player, out var position);
            if (position is null) continue;

            // TODO: make system for validation
            em.SetComponent(player, new Velocity(submitted.Position - position.Coordinates));
        }
    }

    private static Velocity CalculateVelocity(Direction direction, Abilities abilities, float deltaTime)
    {
        var directionVector = direction.Vector;
        var speedValue = abilities.Speed;
        return new Velocity(Vector2.Normalize(directionVector) * speedValue * deltaTime);
    }
}