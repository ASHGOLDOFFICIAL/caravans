using System.Collections.Immutable;
using CaravansCore.Entities.Components.Types;

namespace CaravansCore.Entities.Components;

public record AttackTargetPreferences(ImmutableList<EntityId> Types) : IComponent;