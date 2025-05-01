using CaravansCore.Entities.Components.Types;

namespace CaravansCore.Entities.Components;

public record PlayerConnection(Guid ClientId) : IComponent
{
    public ConnectionState State { get; init; } = ConnectionState.AwaitingInitialSync;
    public bool Spawned { get; init; }
    public bool WantToRespawn { get; init; }
}