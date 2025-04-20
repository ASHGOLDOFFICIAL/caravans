using System.Numerics;
using CaravansCore.Entities.Components;

namespace CaravansCore.Networking;

public readonly record struct EntitySnapshot(
    EntityId Id,
    Guid Uuid,
    Vector2 Position,
    int? Speed
);