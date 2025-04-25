using System.Collections.Immutable;
using CaravansCore.Entities.Components.Types;

namespace CaravansCore.Entities.Components;

public record AccompanyPreferences(ImmutableList<EntityId> Types) : IComponent;