using CaravansCore.Entities.Components;
using CaravansCore.Entities.Components.Types;

namespace CaravansCore.Entities.Systems;

public class GoalSelectionSystem : ISystem
{
    private readonly Random _random = new();
    
    public void Update(EntityManager em, float deltaTime)
    {
        foreach (var entity in em.GetAllEntitiesWith<GoalSet>())
        {
            em.TryGetComponent<GoalSet>(entity, out var goals);
            if (goals is null) continue;

            var possible = Variants(em, entity, goals.Goals);
            var chosen = possible[_random.Next(possible.Count)];
            SelectGoal(em, entity, chosen);
        }
    }

    private static List<GoalType> Variants(EntityManager em, Entity entity, IList<Goal> goals)
    {
        return goals
            .Where(g => CanPerform(em, entity, g.Type))
            .GroupBy(g => g.Priority)
            .FirstOrDefault()?
            .Select(g => g.Type)
            .ToList() ?? [];
    }

    private static bool CanPerform(EntityManager em, Entity entity, GoalType goalType)
    {
        return goalType switch
        {
            GoalType.MoveToTarget => true,
            GoalType.FollowEntity => true,
            _ => false
        };
    }

    private static void SelectGoal(EntityManager em, Entity entity, GoalType goalType)
    {
        switch (goalType)
        {
            case GoalType.MoveToTarget:
                em.SetComponent(entity, new NeedsTargetTile());
                break;
            case GoalType.FollowEntity:
                em.SetComponent(entity, new NeedsFollowTarget());
                break;
        }
    }
}