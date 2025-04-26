using CaravansCore.Entities.Components;

namespace CaravansCore.Entities.Systems;

public class DamageSystem : ISystem
{
    public void Update(EntityManager em, float deltaTime)
    {
        foreach (var (entity, damages) in em.GetAllEntitiesWith<Damages>())
        {
            em.TryGetComponent<Health>(entity, out var health);
            if (health is null)
            {
                em.RemoveComponent<Damages>(entity);
                continue;
            }

            var total = damages.Value
                .Aggregate(0, (current, damage) => current + damage.Amount);
            em.SetComponent(entity, health.Damage(total));
            
            if (!health.IsAlive())
            {
                em.SetComponent(entity, new Death());
                return;
            }
            
            em.RemoveComponent<Damages>(entity);
        }
    }
}