using CaravansCore.Entities.Components;
using Godot;

namespace CaravansCore.Entities.Systems;

public class DeathSystem : ISystem
{
    public void Update(EntityManager em, float deltaTime)
    {
        foreach (var (entity, _) in em.GetAllEntitiesWith<Death>())
        {
            GD.Print($"Died {entity.Uuid}");
            em.RemoveComponent<Death>(entity);
        }
    }
}