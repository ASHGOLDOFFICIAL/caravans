using CaravansCore.Entities.Components;
using CaravansCore.Entities.Components.Types;
using Godot;

namespace CaravansCore.Entities.Systems;

public class GoalSelectionSystem : ISystem
{
    public void Update(EntityManager em, float deltaTime)
    {
        foreach (var (entity, goals) in em.GetAllEntitiesWith<GoalSet>())
        {
            foreach (var _ in goals.Goals.Where(goal => TryToPerform(em, entity, goal.Type)))
            {
                break;
            }
            em.RemoveComponent<LookAroundResult>(entity);
        }
    }

    private static bool TryToPerform(EntityManager em, Entity entity, GoalType goal)
    {
        return goal switch
        {
            GoalType.AccompanyEntity => TryAccompanyEntity(em, entity),
            GoalType.AttackEntity => TryAttackEntity(em, entity),
            GoalType.MoveToTarget => TryMoveToTarget(em, entity),
            _ => false
        };
    }
    
    private static bool TryAccompanyEntity(EntityManager em, Entity entity)
    {
        em.TryGetComponent<AccompanyTarget>(entity, out var target);
        if (target is null) return false;
        em.SetComponent(entity, new CurrentGoal(GoalType.AccompanyEntity));
        GD.Print($"Accompany {entity.Uuid} -> {target.Entity.Uuid}");
        return true;
    }

    private static bool TryAttackEntity(EntityManager em, Entity entity)
    {
        em.TryGetComponent<LookAroundResult>(entity, out var result);
        if (result is null) return false;
        em.TryGetComponent<AttackTargetPreferences>(entity, out var enemies);
        if (enemies is null) return false;

        foreach (var seen in result.SeenEntities)
        {
            em.TryGetComponent<EntityType>(seen, out var type);
            if (type is null) continue;
            if (!enemies.Types.Contains(type.Id)) continue;
            
            em.SetComponent(entity, new AttackTarget(seen));
            em.SetComponent(entity, new CurrentGoal(GoalType.AttackEntity));
            GD.Print($"Attack {entity.Uuid} -> {seen.Uuid}");
            return true;
        }

        return false;
    }

    private static bool TryMoveToTarget(EntityManager em, Entity entity)
    {
        em.SetComponent(entity, new CurrentGoal(GoalType.MoveToTarget));
        em.SetComponent(entity, new NeedsTargetTile());
        return true;
    }
}