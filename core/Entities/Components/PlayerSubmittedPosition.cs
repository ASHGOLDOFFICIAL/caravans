using System.Numerics;

namespace CaravansCore.Entities.Components;

public record PlayerSubmittedPosition(Vector2 Position) : IComponent;