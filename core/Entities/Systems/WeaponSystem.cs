using CaravansCore.Entities.Components;
using CaravansCore.Entities.Components.Types;
using CaravansCore.Utils;
using Aabb = CaravansCore.Utils.Aabb;
using Vector2 = System.Numerics.Vector2;

namespace CaravansCore.Entities.Systems;

public class WeaponSystem : ISystem
{
    private readonly List<Type> _requiredComponentTypes =
        [typeof(AttackIntent), typeof(Cooldown), typeof(Rotation), typeof(CollisionBox), typeof(Position)];
    private readonly List<Type> _requiredComponentTypesToBeAttacked =
        [typeof(CollisionBox), typeof(Position)];

    public void Update(EntityManager em, float deltaTime)
    {
        foreach (var (entity, components) in em.GetAllEntitiesWith(_requiredComponentTypes))
        {
            var cooldown = (Cooldown)components[typeof(Cooldown)];
            em.SetComponent(entity, cooldown.Update(deltaTime));
            if (!cooldown.IsEnded()) continue;
            em.SetComponent(entity, cooldown.Reset());
            
            var rotation = (Rotation)components[typeof(Rotation)];
            var box = (CollisionBox)components[typeof(CollisionBox)];
            var position = (Position)components[typeof(Position)];

            var center = position.Coordinates + Vector2.Normalize(rotation.Direction) * box.Size / 2; 
            var weaponAabb = new Aabb(center, new CollisionBox(2, 1));
            var damage = new Damage(entity, 5, DamageType.Melee);

            var potentialTargets = em
                .GetAllEntitiesWith(_requiredComponentTypesToBeAttacked)
                .Where(t => t.Item1 != entity);
            foreach (var (target, componentsB) in potentialTargets)
            {
                var positionB = (Position)componentsB[typeof(Position)];
                var boxB = (CollisionBox)componentsB[typeof(CollisionBox)];
                
                var bAabb = new Aabb(positionB.Coordinates, boxB);
                var collision = AabbCollisionTools.AabbOverlapsAabb(weaponAabb, bAabb);
                if (!collision) continue;
                
                var damages = em.GetComponentOrSet(target, new Damages([]));
                damages.Value.Enqueue(damage);
            }
            em.RemoveComponent<AttackIntent>(entity);
        }
    }
}