namespace CaravansCore.Entities.Systems;

internal interface ISystem
{
    public void Update(EntityManager em, float deltaTime);
}