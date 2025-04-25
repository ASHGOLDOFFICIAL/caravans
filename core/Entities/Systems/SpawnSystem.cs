using System.Collections.Immutable;
using CaravansCore.Entities.Components;
using CaravansCore.Level;
using CaravansCore.Level.Content;
using CaravansCore.Utils;
using Godot;
using Vector2 = System.Numerics.Vector2;

namespace CaravansCore.Entities.Systems;

internal class SpawnSystem(Layout level) : ISystem
{
    private const float SpawnInterval = 2f;
    private readonly Random _random = new();
    private readonly object _timeLock = new();
    private bool _caravanSpawned;
    private float _timePassed;

    public void Update(EntityManager em, float deltaTime)
    {
        SpawnPlayers(em);
        lock (_timeLock)
        {
            _timePassed += deltaTime;
            if (_timePassed < SpawnInterval) return;
            _timePassed = 0;
        }

        if (!_caravanSpawned)
            SpawnCaravan(em);
    }

    private static void SpawnPlayers(EntityManager em)
    {
        foreach (var entity in em.GetAllEntitiesWith<PlayerConnection>())
        {
            em.TryGetComponent<PlayerConnection>(entity, out var connection);
            if (connection is null || connection.Spawned) continue;
            em.SetComponent(entity, connection with { Spawned = true });
            var position = new Position(new Vector2(10, 15));
            em.SetComponent(entity, position);
            GD.Print($"Spawn player {entity} at {position.Coordinates}");
        }
    }

    private void SpawnCaravan(EntityManager em)
    {
        var entity = EntityFactory.RequestCaravan(em);
        
        em.TryGetComponent<PreferredSpawnTerrain>(entity, out var preferredTiles);
        if (preferredTiles is null)
        {
            em.RemoveEntity(entity);
            return;
        }

        TryFindSpawnPosition(preferredTiles.Tiles, out var point);
        if (point is not {} position)
        {
            em.RemoveEntity(entity);
            return;
        }
        
        Point2D? forGuardian = null;
        var possible = TileTools
            .Neighbors(position)
            .Where(p => level.GetObject(p) == null);
        
        foreach (var tile in possible)
        {
            forGuardian = tile;
        }

        if (forGuardian is {} forGuardianNonNull)
        {
            var guardian = EntityFactory.RequestGuardian(em);
            em.SetComponent(guardian, new FollowedEntity(entity));
            em.SetComponent(guardian, new Position(forGuardianNonNull.ToVector2()));
        }

        _caravanSpawned = true;
        em.SetComponent(entity, new Position(position.ToVector2()));
        GD.Print($"Spawn caravan {entity} at {position}");
    }

    private void TryFindSpawnPosition(
        ImmutableHashSet<TerrainId> preferredTiles,
        out Point2D? position)
    {
        position = null;
        
        var startX = _random.Next(0, level.Width);
        var startY = _random.Next(0, level.Height);

        foreach (var tile in preferredTiles)
        foreach (var x in Enumerable.Range(startX, level.Width))
        foreach (var y in Enumerable.Range(startY, level.Height))
        {
            var point = new Point2D(x, y);
            if (level.GetTerrain(point) != tile) continue;
            position = point;
            return;
        }
    }
}