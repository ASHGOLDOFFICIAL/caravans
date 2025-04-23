using CaravansCore.Level.Content;

namespace CaravansCore.Networking;

public readonly record struct WorldSnapshot(
    int Width,
    int Height,
    TerrainId[]? Terrain,
    ObjectId?[] Objects,
    EntitySnapshot[]? Entities
);