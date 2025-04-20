namespace CaravansCore.Networking;

public readonly record struct Snapshot(
    PlayerSnapshot? Player,
    WorldSnapshot? World
);