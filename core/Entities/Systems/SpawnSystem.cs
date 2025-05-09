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
    private readonly EntityFactory _entityFactory = new();
    private const float SpawnInterval = 2f;
    private readonly Random _random = new();
    private readonly object _timeLock = new();
    private bool _caravanSpawned;
    private float _timePassed;
    private readonly Vector2 _spawnPoint = new(10, 15);

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

    private void SpawnPlayers(EntityManager em)
    {
        foreach (var (entity, connection) in em.GetAllEntitiesWith<PlayerConnection>())
        {
            if (connection.WantToRespawn)
            {
                em.UpdateComponent<PlayerConnection>(entity, 
                    c => c with { WantToRespawn = false });
                if (!em.RemoveComponent<Died>(entity))
                    continue;
                em.RemoveComponent<PlayerSubmittedPosition>(entity);
                em.UpdateComponent<Health>(entity, h => h.Full());

                em.SetComponent(entity, new Position(_spawnPoint));
                GD.Print($"Respawn player {entity.Uuid} at {_spawnPoint}");
                continue;
            }

            if (connection.Spawned) continue;
            
            em.UpdateComponent<PlayerConnection>(
                entity, 
                c => c with { Spawned = true });
            em.SetComponent(entity, new Position(_spawnPoint));
            GD.Print($"Spawn player {entity.Uuid} at {_spawnPoint}");
        }
    }

    private void SpawnCaravan(EntityManager em)
    {
        var caravan = _entityFactory.RequestCaravan(em);
        
        em.TryGetComponent<PreferredSpawnTerrain>(caravan, out var preferredTiles);
        if (preferredTiles is null)
        {
            em.RemoveEntity(caravan);
            return;
        }

        TryFindSpawnPosition(preferredTiles.Tiles, out var point);
        if (point is not {} position)
        {
            em.RemoveEntity(caravan);
            return;
        }
        
        var possible = TileTools
            .Neighbors(position)
            .Where(p => level.GetObject(p) == null);
        
        foreach (var tile in possible)
        {
            var guardian = _entityFactory.RequestGuardian(em);
            em.SetComponent(guardian, new AccompanyTarget(caravan));
            em.SetComponent(guardian, new Position(tile.ToVector2()));
            break;
        }

        _caravanSpawned = true;
        em.SetComponent(caravan, new Position(position.ToVector2()));
        GD.Print($"Spawn caravan {caravan} at {position}");
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