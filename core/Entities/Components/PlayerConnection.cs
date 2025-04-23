using CaravansCore.Entities.Components.Types;

namespace CaravansCore.Entities.Components;

public record PlayerConnection(
    Guid ClientId,
    ConnectionState State = ConnectionState.AwaitingInitialSync,
    bool Spawned = false
) : IComponent;