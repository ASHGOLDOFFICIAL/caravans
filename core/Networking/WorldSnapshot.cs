using CaravansCore.Level.Content;
using CaravansCore.Utils;

namespace CaravansCore.Networking;

public readonly record struct WorldSnapshot(
    int Width,
    int Height,
    TerrainId[]? Terrain,
    ObjectId?[] Objects,
    EntitySnapshot[]? Entities
);