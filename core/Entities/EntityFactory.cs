using System.Numerics;
using CaravansCore.Entities.Components;
using CaravansCore.Entities.Components.Types;
using CaravansCore.Level.Content;

namespace CaravansCore.Entities;

public class EntityFactory
{
    private readonly Random _random = new();
    
    internal Entity RequestPlayer(EntityManager manager, Guid clientId)
    {
        var entity = manager.CreateEntity();
        manager.SetComponent(entity, new EntityType(EntityId.Player));
        manager.SetComponent(entity, new Networked());
        
        manager.SetComponent(entity, new Abilities(20));
        manager.SetComponent(entity, new CollisionBox(0.5f, 0.5f));
        
        manager.SetComponent(entity, new Cooldown(0.5f));
        manager.SetComponent(entity, new Health(20));
        manager.SetComponent(entity, new Score(0));
        
        manager.SetComponent(entity, new PlayerConnection(clientId));
        return entity;
    }

    internal Entity RequestCaravan(EntityManager manager)
    {
        var entity = manager.CreateEntity();
        manager.SetComponent(entity, new EntityType(EntityId.Caravan));
        manager.SetComponent(entity, new Networked());
        
        manager.SetComponent(entity, new Abilities(1));
        manager.SetComponent(entity, new CollisionBox(1, 1));
        manager.SetComponent(entity, new Rotation(-Vector2.UnitY));

        manager.SetComponent(entity, new Health(100));
        manager.SetComponent(entity, new Score(_random.Next(10, 100)));
        
        manager.SetComponent(entity, new GoalSet([
            new Goal(1, GoalType.MoveToTarget)
        ]));

        manager.SetComponent(entity, new PreferredSpawnTerrain([TerrainId.City]));
        manager.SetComponent(entity, new TargetPositionPreference(TargetingPolicy.Random, [TerrainId.City]));
        manager.SetComponent(entity, new PathPreference([TerrainId.Path, TerrainId.City]));

        return entity;
    }

    internal Entity RequestGuardian(EntityManager manager)
    {
        var entity = manager.CreateEntity();
        manager.SetComponent(entity, new EntityType(EntityId.Guardian));
        manager.SetComponent(entity, new Networked());
        
        manager.SetComponent(entity, new Abilities(2));
        manager.SetComponent(entity, new CollisionBox(0.5f, 0.5f));
        manager.SetComponent(entity, new FieldOfView(5, 90));
        manager.SetComponent(entity, new Rotation(-Vector2.UnitY));
        
        manager.SetComponent(entity, new Health(10));
        manager.SetComponent(entity, new Cooldown(0.5f));
        manager.SetComponent(entity, new Score(_random.Next(10, 50)));
        
        manager.SetComponent(entity, new GoalSet([
            new Goal(1, GoalType.AttackEntity),
            new Goal(2, GoalType.AccompanyEntity),
            new Goal(3, GoalType.MoveToTarget)
        ]));
        
        manager.SetComponent(entity, new TargetPositionPreference(TargetingPolicy.Random, [TerrainId.City]));
        manager.SetComponent(entity, new AttackTargetPreferences([EntityId.Player]));
        manager.SetComponent(entity, new AccompanyPreferences([EntityId.Caravan]));

        return entity;
    }
}