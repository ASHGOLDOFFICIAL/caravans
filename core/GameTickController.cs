using System.Timers;
using CaravansCore.Entities;
using CaravansCore.Entities.Systems;
using Timer = System.Timers.Timer;

namespace CaravansCore;

internal class GameTickController(GameServer server)
{
    private const double Interval = 50; // 20 TPS
    private readonly ClientSyncSystem _clientSyncSystem = new(server.World, server.Clients);
    private readonly CollisionDetectionSystem _collisionDetectionSystem = new(server.World.Layout);
    private readonly CollisionResolutionSystem _collisionResolutionSystem = new();
    private readonly EntityManager _entityManager = server.World.EntityManager;
    private readonly InteractionSystem _interactionSystem = new();
    private readonly PathfindingSystem _pathfindingSystem = new(server.World.Layout);
    private readonly SpawnSystem _spawnSystem = new(server.World.Layout);
    private readonly TargetChoosingSystem _targetChoosingSystem = new(server.World.Layout);
    private readonly Timer _timer = new(Interval);
    private readonly VelocityApplicationSystem _velocityApplicationSystem = new();
    private readonly VelocityCalculationSystem _velocityCalculationSystem = new();
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

        // Spawn entities
        _spawnSystem.Update(_entityManager, delta);
        // Chose target for them
        _targetChoosingSystem.Update(_entityManager, delta);
        // Find path to target
        _pathfindingSystem.Update(_entityManager, delta);
        // Calculate velocity
        _velocityCalculationSystem.Update(_entityManager, delta);
        // Check for collision along desired velocity
        _collisionDetectionSystem.Update(_entityManager, delta);
        // Resolve collisions by clamping desired velocity
        _collisionResolutionSystem.Update(_entityManager, delta);
        // Apply clamped velocity
        _velocityApplicationSystem.Update(_entityManager, delta);
        // Check for interactions
        _interactionSystem.Update(_entityManager, delta);
        // Send world state to clients
        _clientSyncSystem.Update(_entityManager, delta);
    }
}