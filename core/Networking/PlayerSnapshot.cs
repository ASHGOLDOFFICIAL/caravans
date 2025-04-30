namespace CaravansCore.Networking;

public readonly record struct PlayerSnapshot(
    EntitySnapshot AttachedEntity,
    int Score,
    bool IsDead = false
);