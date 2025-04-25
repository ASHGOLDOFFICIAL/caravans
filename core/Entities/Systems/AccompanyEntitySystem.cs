using System.Numerics;
using CaravansCore.Entities.Components;
using CaravansCore.Entities.Components.Types;

namespace CaravansCore.Entities.Systems;

public class AccompanyEntitySystem : ISystem
{
    private readonly List<Type> _requiredComponentTypes =
        [typeof(CurrentGoal), typeof(AccompanyTarget), typeof(Position)];

    public void Update(EntityManager em, float deltaTime)
    {
        foreach (var (entity, components) in em.GetAllEntitiesWith(_requiredComponentTypes))
        {
            var goal = (CurrentGoal)components[typeof(CurrentGoal)];
            if (goal.Goal != GoalType.AccompanyEntity) continue;
            
            var target = (AccompanyTarget)components[typeof(AccompanyTarget)];
            var position = (Position)components[typeof(Position)];
            
            em.TryGetComponent<Position>(target.Entity, out var targetPosition);
            if (targetPosition is null) continue;
            
            var desired = targetPosition.Coordinates + RelativePositionToTarget(em, target.Entity) * 2;
            em.SetComponent(entity, new TargetPosition(desired));
        }
    }

    private static Vector2 RelativePositionToTarget(EntityManager em, Entity target)
    {
        em.TryGetComponent<Direction>(target, out var targetDirection);
        if (targetDirection is null) return Vector2.Zero;
        var rotation = Matrix3x2.CreateRotation((float)Math.PI / 2);
        return Vector2.Transform(Vector2.Normalize(targetDirection.Vector), rotation);
    }
}
