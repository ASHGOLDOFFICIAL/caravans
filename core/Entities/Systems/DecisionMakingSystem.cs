using CaravansCore.Entities.Components;
using CaravansCore.Entities.Components.Types;
using Godot;

namespace CaravansCore.Entities.Systems;

public class DecisionMakingSystem : ISystem
{
    public void Update(EntityManager em, float deltaTime)
    {
        foreach (var entity in em.GetAllEntitiesWith<LookAroundResult>())
        {
            // TODO: decide
            em.TryGetComponent<LookAroundResult>(entity, out var result);
            if (result is null) continue;

            foreach (var seen in result.SeenEntities)
            {
                GD.Print($"{entity} seen {seen}");
                em.SetComponent(entity, new InteractionRequest(seen, InteractionType.Default));
            }
            foreach (var point in result.SeenObjects)
            {
                // GD.Print($"{entity} seen {point}");
            }

            em.RemoveComponent<LookAroundResult>(entity);
        }
    }
}