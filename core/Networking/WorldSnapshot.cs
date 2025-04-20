using CaravansCore.Objects;
using CaravansCore.Terrain;
using CaravansCore.Utils;

namespace CaravansCore.Networking;

public readonly record struct WorldSnapshot(
    int Width,
    int Height,
    TerrainId[]? Terrain,
    Dictionary<Point2D, ObjectId>? Objects,
    EntitySnapshot[]? Entities
);