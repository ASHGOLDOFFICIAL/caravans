using CaravansCore.Entities.Components.Types;

namespace CaravansCore.Entities.Components;

internal record Damages(Queue<Damage> Value) : IComponent;