using CaravansCore.Entities.Components;
using CaravansCore.Level;
using CaravansCore.Networking;

namespace CaravansCore.Entities.Systems;

internal class ClientSyncSystem(World level, Dictionary<Guid, IClient> clients) : ISystem
{
    private readonly object _stateCheckLock = new();

    public void Update(EntityManager em, float deltaTime)
    {
        foreach (var entity in em.GetAllEntitiesWith<PlayerConnection>())
            lock (_stateCheckLock)
            {
                em.TryGetComponent<PlayerConnection>(entity, out var connection);
                if (connection is null) continue;
                clients.TryGetValue(connection.ClientId, out var client);
                if (client is null) continue;
                var snapshot = BuildSnapshotForClient(
                    entity, em,
                    connection.State == ConnectionState.AwaitingInitialSync);
                em.SetComponent(entity, connection with { State = ConnectionState.Synced });
                client.Receive(snapshot);
            }
    }

    private Snapshot BuildSnapshotForClient(
        Entity client,
        EntityManager em,
        bool initial = false
    )
    {
        var playerSnapshot = BuildPlayerSnapshot(em, client);
        var worldSnapshot = BuildWorldSnapshot(em, initial);
        var snapshot = new Snapshot(playerSnapshot, worldSnapshot);
        return snapshot;
    }

    private static PlayerSnapshot? BuildPlayerSnapshot(EntityManager em, Entity client)
    {
        var snapshot = BuildEntitySnapshot(em, client);
        em.TryGetComponent<Score>(client, out var scoreComponent);
        var score = scoreComponent ?? new Score(0);

        if (snapshot is { } nonNullSnapshot)
            return new PlayerSnapshot(nonNullSnapshot, score.Value);
        return null;
    }

    private WorldSnapshot BuildWorldSnapshot(EntityManager em, bool initial)
    {
        var terrain = initial ? level.Layout.GetTerrainLayer() : null;
        var objects = level.Layout.GetObjects();
        var entities = BuildEntitySnapshots(em);
        return new WorldSnapshot(level.Layout.Width, level.Layout.Height, terrain, objects, entities);
    }

    private static EntitySnapshot[] BuildEntitySnapshots(EntityManager em)
    {
        List<EntitySnapshot> snapshots = [];
        foreach (var entity in em.GetAllEntitiesWith<Networked>())
        {
            var snapshot = BuildEntitySnapshot(em, entity);
            if (snapshot is { } nonNullSnapshot) snapshots.Add(nonNullSnapshot);
        }

        return snapshots.ToArray();
    }

    private static EntitySnapshot? BuildEntitySnapshot(EntityManager em, Entity entity)
    {
        em.TryGetComponent<Position>(entity, out var position);
        if (position is null) return null;
        em.TryGetComponent<EntityType>(entity, out var id);
        if (id is null) return null;

        em.TryGetComponent<Abilities>(entity, out var abilities);
        var snapshot = new EntitySnapshot(id.Id, entity.Uuid, position.Value, abilities?.Speed);
        return snapshot;
    }
}