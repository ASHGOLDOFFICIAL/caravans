using CaravansCore.Entities.Components;

namespace CaravansCore.Entities.Systems;

public class DeathSystem : ISystem
{
    private readonly List<Type> _types = [ typeof(Died), typeof(Score) ];
    
    public void Update(EntityManager em, float deltaTime)
    {
        foreach (var (_, components) in em.GetAllEntitiesWith(_types))
        {
            var score = (Score)components[typeof(Score)];
            var death = (Died)components[typeof(Died)];
            var killer = death.Killer;
            if (killer is null) continue;
            
            em.UpdateComponent<Score>(killer, old => old.Add(score.Value));
        }
    }
}