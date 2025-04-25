using CaravansCore.Entities.Components;
using CaravansCore.Entities.Components.Types;

namespace CaravansCore.Entities.Systems;

public class AttackEntitySystem : ISystem
{
    private readonly List<Type> _requiredComponentTypes =
        [typeof(CurrentGoal), typeof(AttackTarget), typeof(Position)];

    public void Update(EntityManager em, float deltaTime)
    {
        foreach (var (entity, components) in em.GetAllEntitiesWith(_requiredComponentTypes))
        {
            var goal = (CurrentGoal)components[typeof(CurrentGoal)];
            if (goal.Goal != GoalType.AttackEntity) continue;
            
            var target = (AttackTarget)components[typeof(AttackTarget)];
            var position = (Position)components[typeof(Position)];
            
            em.TryGetComponent<Position>(target.Entity, out var targetPosition);
            if (targetPosition is null) continue;
            
            em.SetComponent(entity, new Direction(targetPosition.Coordinates - position.Coordinates));
        }
    }
}
