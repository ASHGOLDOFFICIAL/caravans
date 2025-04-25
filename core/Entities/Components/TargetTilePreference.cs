using System.Collections.Immutable;
using CaravansCore.Entities.Components.Types;
using CaravansCore.Level.Content;

namespace CaravansCore.Entities.Components;

public record TargetTilePreference(
    TargetingPolicy Policy,
    ImmutableHashSet<TerrainId> Preferred
) : IComponent;