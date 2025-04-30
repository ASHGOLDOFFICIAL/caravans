using CaravansCore.Entities.Components;

namespace CaravansCore.Entities.Systems;

public class DeathSystem : ISystem
{
    private readonly List<Type> _types = [ typeof(Death), typeof(Score) ];
    
    public void Update(EntityManager em, float deltaTime)
    {
        foreach (var (_, components) in em.GetAllEntitiesWith(_types))
        {
            var score = (Score)components[typeof(Score)];
            var death = (Death)components[typeof(Death)];
            var killer = death.Killer;
            if (killer is null) continue;
            
            em.UpdateComponent<Score>(killer, old => old.Add(score.Value));
        }
    }
}