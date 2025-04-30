using CaravansCore.Entities;
using CaravansCore.Level;
using CaravansCore.Networking;

namespace CaravansCore;

public class GameServer
{
    private readonly EntityFactory _entityFactory = new();
    
    public GameServer()
    {
        var tickController = new GameTickController(this);
        tickController.Start();
    }

    internal World World { get; } = new();
    internal Dictionary<Guid, IClient> Clients { get; } = [];

    public PlayerController Connect(IClient client)
    {
        var uuid = Guid.NewGuid();
        Clients.Add(uuid, client);
        var player = _entityFactory.RequestPlayer(World.EntityManager, uuid);
        var controller = new PlayerController(player, World);
        return controller;
    }
}