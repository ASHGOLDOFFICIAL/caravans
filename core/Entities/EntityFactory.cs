using System.Numerics;
using CaravansCore.Entities.Components;
using CaravansCore.Entities.Components.Types;
using CaravansCore.Level.Content;

namespace CaravansCore.Entities;

public static class EntityFactory
{
    internal static Entity RequestPlayer(EntityManager manager, Guid clientId)
    {
        var entity = manager.CreateEntity();
        manager.SetComponent(entity, new EntityType(EntityId.Player));
        manager.SetComponent(entity, new Networked());
        manager.SetComponent(entity, new PlayerConnection(clientId));
        manager.SetComponent(entity, new Abilities(20));
        manager.SetComponent(entity, new CollisionBox(0.5f, 0.5f));
        manager.SetComponent(entity, new Score(0));
        return entity;
    }

    internal static Entity RequestCaravan(EntityManager manager)
    {
        var entity = manager.CreateEntity();
        manager.SetComponent(entity, new EntityType(EntityId.Caravan));
        manager.SetComponent(entity, new Networked());
        manager.SetComponent(entity, new CollisionBox(1, 1));

        manager.SetComponent(entity, new Abilities(1));
        manager.SetComponent(entity, new FieldOfView(5, 45));
        manager.SetComponent(entity, new Rotation(Vector2.UnitX));

        manager.SetComponent(entity, new PreferredSpawnTerrain([TerrainId.City]));
        manager.SetComponent(entity, new TargetTilePreference(TargetingPolicy.Random, [TerrainId.City]));
        manager.SetComponent(entity, new PathPreference([TerrainId.Path, TerrainId.City]));

        manager.SetComponent(entity, new GoalSet([
            new Goal(1, GoalType.MoveToTarget)
        ]));

        manager.SetComponent(entity, new Score(100));
        return entity;
    }

    internal static Entity RequestGuardian(EntityManager manager)
    {
        var entity = manager.CreateEntity();
        manager.SetComponent(entity, new EntityType(EntityId.Guardian));
        manager.SetComponent(entity, new Networked());
        manager.SetComponent(entity, new CollisionBox(0.5f, 0.5f));

        manager.SetComponent(entity, new Abilities(5));
        manager.SetComponent(entity, new FieldOfView(5, 45));
        manager.SetComponent(entity, new Rotation(Vector2.UnitX));
        
        manager.SetComponent(entity, new TargetTilePreference(TargetingPolicy.Random, [TerrainId.City]));

        manager.SetComponent(entity, new GoalSet([
            new Goal(1, GoalType.FollowEntity),
            new Goal(2, GoalType.MoveToTarget)
        ]));

        manager.SetComponent(entity, new Score(25));
        return entity;
    }
}