using CaravansCore.Entities.Components;
using Vector2 = System.Numerics.Vector2;

namespace CaravansCore.Entities.Systems;

internal class VelocityCalculationSystem : ISystem
{
    private readonly HashSet<Type> _requiredComponentTypes = [typeof(Direction), typeof(Abilities)];

    public void Update(EntityManager em, float deltaTime)
    {
        foreach (var entity in em.GetAllEntitiesWith(_requiredComponentTypes))
        {
            em.TryGetComponent<Direction>(entity, out var direction);
            if (direction is null || direction.Vector == Vector2.Zero) continue;

            em.TryGetComponent<Abilities>(entity, out var abilities);
            if (abilities is null) continue;

            var velocity = CalculateVelocity(direction, abilities, deltaTime);
            em.SetComponent(entity, velocity);
        }

        foreach (var player in em.GetAllEntitiesWith<PlayerSubmittedPosition>())
        {
            em.TryGetComponent<Position>(player, out var position);
            if (position is null) continue;
            em.TryGetComponent<PlayerSubmittedPosition>(player, out var submitted);
            if (submitted is null) continue;

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