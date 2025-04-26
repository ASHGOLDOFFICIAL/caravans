using CaravansCore.Entities.Components;
using CaravansCore.Entities.Components.Types;
using Vector2 = System.Numerics.Vector2;

namespace CaravansCore.Entities.Systems;

public class AttackEntityGoalSystem : ISystem
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

            if (!InRange(position.Coordinates, targetPosition.Coordinates))
            {
                Chase(em, entity, position.Coordinates, targetPosition.Coordinates);
                continue;
            }

            Attack(em, entity);
        }
    }

    private static bool InRange(Vector2 position, Vector2 targetPosition)
    {
        return Vector2.DistanceSquared(position, targetPosition) <= 1;
    }

    private static void Attack(EntityManager em, Entity entity)
    {
        em.SetComponent(entity, new AttackIntent());
    }

    private static void Chase(EntityManager em, Entity entity, Vector2 position, Vector2 targetPosition)
    {
        em.SetComponent(entity, new TargetPosition(targetPosition));
        em.SetComponent(entity, new MoveIntent());
    }
}
