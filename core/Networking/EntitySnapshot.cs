using System.Numerics;
using CaravansCore.Entities.Components.Types;

namespace CaravansCore.Networking;

public readonly record struct EntitySnapshot(
    EntityId Id,
    Guid Uuid,
    Vector2 Position,
    Vector2? Rotation,
    int? Speed
);