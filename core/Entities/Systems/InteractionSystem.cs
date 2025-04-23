using CaravansCore.Entities.Components;
using CaravansCore.Entities.Components.Types;

namespace CaravansCore.Entities.Systems;

public class InteractionSystem : ISystem
{
    public void Update(EntityManager em, float deltaTime)
    {
        foreach (var entity in em.GetAllEntitiesWith<InteractionRequest>())
        {
            em.TryGetComponent<InteractionRequest>(entity, out var request);
            if (request is null) continue;

            HandleInteraction(em, entity, request.TargetEntity, request.Type);
            em.RemoveComponent<InteractionRequest>(entity);
        }
    }

    private static void HandleInteraction(EntityManager em, Entity user, Entity target, InteractionType type)
    {
        switch (type)
        {
            default:
            case InteractionType.Default:
                em.TryGetComponent<Score>(target, out var targetScore);
                if (targetScore is null) break;
                em.TryGetComponent<Score>(user, out var userScore);
                if (userScore is null) break;
                var newUserScore = userScore.Value + targetScore.Value;
                em.SetComponent(user, new Score(newUserScore));
                em.SetComponent(target, new Score(0));
                break;
        }
    }
}