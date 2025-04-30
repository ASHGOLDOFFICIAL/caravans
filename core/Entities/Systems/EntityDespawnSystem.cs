using CaravansCore.Entities.Components;

namespace CaravansCore.Entities.Systems;

public class EntityDespawnSystem : ISystem
{
    public void Update(EntityManager em, float deltaTime)
    {
        DespawnDead(em);
    }

    private static void DespawnDead(EntityManager em)
    {
        foreach (var (entity, _) in em.GetAllEntitiesWith<Death>())
        {
            if (em.TryGetComponent<PlayerConnection>(entity, out _)) return;
            em.RemoveEntity(entity);
        }
    }
}