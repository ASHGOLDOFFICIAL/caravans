using System.Timers;
using CaravansCore.Entities;
using CaravansCore.Entities.Systems;
using Timer = System.Timers.Timer;

namespace CaravansCore;

internal class GameTickController(GameServer server)
{
    private const double Interval = 50; // 20 TPS
    private readonly ClientSyncSystem _clientSyncSystem = new(server.World, server.Clients);
    private readonly CollisionDetectionSystem _collisionDetectionSystem = new();
    private readonly CollisionResolutionSystem _collisionResolutionSystem = new();
    private readonly EntityManager _entityManager = server.World.EntityManager;
    private readonly InteractionSystem _interactionSystem = new();
    private readonly MovementSystem _movementSystem = new();
    private readonly PathfindingSystem _pathfindingSystem = new(server.World.Layout);

    private readonly SpawnSystem _spawnSystem = new(server.World.Layout);
    private readonly TargetChoosingSystem _targetChoosingSystem = new(server.World.Layout);
    private readonly Timer _timer = new(Interval);
    private DateTime _lastSignalTime;

    public void Start()
    {
        _timer.Elapsed += Tick;
        _timer.Enabled = true;
    }

    private void Tick(object? sender, ElapsedEventArgs e)
    {
        var delta = (e.SignalTime - _lastSignalTime).Milliseconds / 1000f;
        _lastSignalTime = e.SignalTime;

        _spawnSystem.Update(_entityManager, delta);
        _targetChoosingSystem.Update(_entityManager, delta);
        _pathfindingSystem.Update(_entityManager, delta);
        _movementSystem.Update(_entityManager, delta);
        _collisionDetectionSystem.Update(_entityManager, delta);
        _collisionResolutionSystem.Update(_entityManager, delta);
        _interactionSystem.Update(_entityManager, delta);
        _clientSyncSystem.Update(_entityManager, delta);
    }
}