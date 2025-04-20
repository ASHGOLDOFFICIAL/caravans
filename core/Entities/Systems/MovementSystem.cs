using CaravansCore.Entities.Components;
using Vector2 = System.Numerics.Vector2;

namespace CaravansCore.Entities.Systems;

internal class MovementSystem : ISystem
{
    private readonly HashSet<Type> _requiredComponentTypes = [typeof(Position), typeof(Velocity), typeof(Abilities)];

    public void Update(EntityManager em, float deltaTime)
    {
        foreach (var entity in em.GetAllEntitiesWith(_requiredComponentTypes))
        {
            em.TryGetComponent<Velocity>(entity, out var velocity);
            if (velocity is null || velocity.Vector == Vector2.Zero) continue;

            em.TryGetComponent<Position>(entity, out var position);
            if (position is null) continue;

            em.TryGetComponent<Abilities>(entity, out var speed);
            if (speed is null) continue;

            var newPos = MoveToward(position, velocity, speed, deltaTime);
            em.SetComponent(entity, newPos);
        }
    }

    private static Position MoveToward(Position pos, Velocity velocity, Abilities abilities, float deltaTime)
    {
        var positionValue = pos.Value;
        var velocityValue = velocity.Vector;
        var speedValue = abilities.Speed;
        return new Position(positionValue + Vector2.Normalize(velocityValue) * speedValue * deltaTime);
    }
}