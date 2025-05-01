using CaravansCore.Entities.Components;

namespace CaravansCore.Entities.Systems;

public class DespawnSystem : ISystem
{
    public void Update(EntityManager em, float deltaTime)
    {
        DespawnDead(em);
    }

    private static void DespawnDead(EntityManager em)
    {
        foreach (var (entity, _) in em.GetAllEntitiesWith<Died>())
        {
            if (em.TryGetComponent<PlayerConnection>(entity, out _))
            {
                // TODO: sketchy
                em.RemoveComponent<Position>(entity);
                continue;
            }

            em.RemoveEntity(entity);
        }
    }
}
