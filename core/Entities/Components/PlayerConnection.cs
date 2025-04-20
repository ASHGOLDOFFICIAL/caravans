namespace CaravansCore.Entities.Components;

public enum ConnectionState
{
    AwaitingInitialSync,
    Synced
}

public record PlayerConnection(
    Guid ClientId,
    ConnectionState State = ConnectionState.AwaitingInitialSync,
    bool Spawned = false
) : IComponent;