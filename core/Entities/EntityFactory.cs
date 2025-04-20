using CaravansCore.Entities.Components;
using CaravansCore.Terrain;

namespace CaravansCore.Entities;

public static class EntityFactory
{
    internal static Entity RequestPlayer(EntityManager manager, Guid clientId)
    {
        var entity = manager.CreateEntity();
        manager.SetComponent(entity, new EntityType(EntityId.Player));
        manager.SetComponent(entity, new Networked());
        manager.SetComponent(entity, new PlayerConnection(clientId));
        manager.SetComponent(entity, new Abilities(10));
        manager.SetComponent(entity, new CollisionBox(1, 1));
        manager.SetComponent(entity, new Score(0));
        return entity;
    }

    internal static Entity RequestCaravan(EntityManager manager)
    {
        var entity = manager.CreateEntity();
        manager.SetComponent(entity, new EntityType(EntityId.Caravan));
        manager.SetComponent(entity, new Networked());
        manager.SetComponent(entity, new Abilities(3));
        manager.SetComponent(entity, new CollisionBox(1, 1));
        manager.SetComponent(entity, new PreferredSpawnTerrain([TerrainId.City]));
        manager.SetComponent(entity, new TargetTilePreference(TargetingPolicy.Random, [TerrainId.City]));
        manager.SetComponent(entity, new PathPreference([TerrainId.Path, TerrainId.City]));
        manager.SetComponent(entity, new Score(100));
        return entity;
    }
}